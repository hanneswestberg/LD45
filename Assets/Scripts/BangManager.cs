using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BangManager : MonoBehaviour {
    // Singleton instance
    public static BangManager instance;

#pragma warning disable 0649
    [Header("Phases:")]
    [Tooltip("All the phases of the game")]
    [SerializeField]
    private List<PhaseData> phaseData = new List<PhaseData>();

    // Current statistic values
    private float currentDensity = 0;
    public float CurrentDensity {
        get {
            return currentDensity;
        }
    }
    private float currentTemperature = 0;
    public float CurrentTemperature {
        get {
            return currentTemperature;
        }
    }
    private float currentBaseVolatility = 0;
    public float CurrentVolatility {
        get {
            return currentBaseVolatility + (CurrentTemperature / 10f);
        }
    }

    // Current masses and ratios
    public float CurrentMassRegular { get; private set; }
    public float CurrentMassDark { get; private set; }
    public float CurrentMassAnti { get; private set; }
    public float CurrentMassGainRate { get; private set; }

    public float CurrentMass {
        get {
            return CurrentMassRegular + CurrentMassDark + CurrentMassAnti;
        }
    }
    public float CurrentMassRatioRegular {
        get {
            return CurrentMassRegular / CurrentMass;
        }
    }
    public float CurrentMassRatioDark {
        get {
            return CurrentMassDark / CurrentMass;
        }
    }
    public float CurrentMassRatioAnti {
        get {
            return CurrentMassAnti / CurrentMass;
        }
    }

    public Phase CurrentPhase { get; private set; }
    public int CurrentPhaseIndex { get; private set; }
    public bool PhaseIsOnGoing { get; private set; }
    public bool IsGameFinishedAndWaitingForUserToExplode { get; private set; }
    public Queue<PhaseData> phaseQueueData;


    [Header("Sounds:")]
    [SerializeField]
    private SimpleAudioEvent startSound;
    [SerializeField]
    private SimpleAudioEvent phaseSound;

    [Header("AudioSources:")]
    [SerializeField]
    private AudioSource startAudioSource;
    [SerializeField]
    private AudioSource phaseAudioSource;

    private float tempMassGainRatio;


    public void StartNewBigBang() {
        IsGameFinishedAndWaitingForUserToExplode = false;

        // Start everything here
        CurrentMassRegular = 0.001f;
        CurrentMassDark = 0.001f;
        CurrentMassAnti = 0.001f;
        currentBaseVolatility = 0f;
        currentDensity = 0;
        currentTemperature = 0;

        // Don't start if we have no phases set
        if(!phaseData.Any())
            return;

        phaseQueueData = new Queue<PhaseData>();
        foreach(var phase in phaseData) {
            phaseQueueData.Enqueue(phase);
        }

        // Generate the phase
        CurrentPhase = GenerateNewPhase(phaseQueueData.Dequeue(), 1f);

        ParticleController.instance.StartParticles();

        // Start the mission
        StartCoroutine(PhaseLoop(CurrentPhase));
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public float CalculateErrorMargin() {
        return ((Mathf.Abs(CurrentPhase.MassRatioRegular - CurrentMassRatioRegular)
            + Mathf.Abs(CurrentPhase.MassRatioAnti - CurrentMassRatioAnti)
            + Mathf.Abs(CurrentPhase.MassRatioDark - CurrentMassRatioDark)) * 100f) 
            * (((CurrentMass/CurrentPhase.TargetMass) < 1) ? 1/(CurrentMass / CurrentPhase.TargetMass) : CurrentMass / CurrentPhase.TargetMass);
    }

    public float CalculateVolatilityGain() {
        var value = CalculateErrorMargin() * (1 + CurrentPhase.ErrorPenalty);
        currentBaseVolatility += value/100f;
        return value;
    }

    /// <summary>
    /// Generate cool new phases
    /// </summary>
    /// <param name="currentPhaseData"></param>
    /// <returns>A new phase</returns>
    private Phase GenerateNewPhase(PhaseData currentPhaseData, float overrideCurrentMass = 0) {
        CurrentPhaseIndex = phaseData.IndexOf(currentPhaseData);
        var model = new  Phase() {
            PhaseName = currentPhaseData.PhaseName,
            TargetMass = (overrideCurrentMass != 0 ? overrideCurrentMass : CurrentMass) * Random.Range(currentPhaseData.NewMassFactor.minValue, currentPhaseData.NewMassFactor.maxValue),
            AcceptedErrorMargin = currentPhaseData.AcceptedErrorMargin,
            ErrorPenalty = Random.Range(currentPhaseData.AcceptedErrorMargin.minValue, currentPhaseData.AcceptedErrorMargin.maxValue),
            MissionText = currentPhaseData.MissionText,
            MissionTime = Random.Range(currentPhaseData.MissionTime.minValue, currentPhaseData.MissionTime.maxValue)
        };

        // Generate a new ratio:
        var totalMassDifference = model.TargetMass - CurrentMass;
        var values = new float[3];
        for(int i = 0; i < 3; i++) {
            values[i] = Random.Range(0, 1f);
        }
        var total = values[0] + values[1] + values[2];

        model.MassRatioRegular = values[0] / total;
        model.MassRatioAnti = values[1] / total;
        model.MassRatioDark = values[2] / total;

        CurrentMassGainRate = ((model.TargetMass - (overrideCurrentMass != 0 ? overrideCurrentMass : CurrentMass)) / model.MissionTime) * 3f;

        return model;
    }

    /// <summary>
    /// The main phase loop
    /// </summary>
    /// <param name="phase"></param>
    /// <returns></returns>
    private IEnumerator PhaseLoop(Phase phase) {

        if(!GameManager.instance.DebugMode) {
            // Wait and show message here
            UIManager.instance.UpdatePanelText($"> start {phase.MissionText}", 2f, true, true);
            yield return new WaitForSeconds(3f);
            UIManager.instance.UpdatePanelText("\n...", 1f, false, false);
            startSound.Play(startAudioSource);

            yield return new WaitForSeconds(2f);

        }

        UIManager.instance.UpdateUI();
        UIManager.instance.UpdatePanelForMission();

        yield return new WaitForSeconds(3f);
        phaseSound.Play(phaseAudioSource);

        yield return new WaitForSeconds(1f);

        PhaseIsOnGoing = true;
        var phaseFailed = false;
        while(PhaseIsOnGoing) {

            if(!UIManager.instance.UserInputPause) {
                // Update mission timer
                phase.MissionTime -= Time.deltaTime;

                // Volatility increases slowly if temperature or density is high
                currentBaseVolatility += Mathf.Clamp((currentTemperature - 0.8f) * Time.deltaTime / 20f, 0f, 1f);
                currentBaseVolatility += Mathf.Clamp((currentDensity - 0.8f) * Time.deltaTime / 20f, 0f, 1f);

                // Densisty increases if the flow is too high
                currentDensity = Mathf.Clamp(currentDensity - (Time.deltaTime / 30f), 0f, 1f);
                if(UIManager.instance.NormalizedUserInputMassGainRatio > 0.65f) {
                    currentDensity = Mathf.Clamp(currentDensity + (Time.deltaTime/10f)* (0.35f + UIManager.instance.NormalizedUserInputMassGainRatio), 0f, 1f);
                }

                // Temperature increases if the flow changes too fast
                currentTemperature = Mathf.Clamp(currentTemperature - (Time.deltaTime / 40f), 0f, 1f);
                if(tempMassGainRatio != UIManager.instance.NormalizedUserInputMassGainRatio) {
                    currentTemperature = Mathf.Clamp(currentTemperature + 0.03f, 0f, 1f);
                }
                tempMassGainRatio = UIManager.instance.NormalizedUserInputMassGainRatio;


                // Normalize input values
                var totInput = (UIManager.instance.NormalizedUserInputRatioRegular + UIManager.instance.NormalizedUserInputRatioDark + UIManager.instance.NormalizedUserInputRatioAnti);

                if(totInput > 0) {
                    CurrentMassRegular += Time.deltaTime * (UIManager.instance.NormalizedUserInputMassGainRatio * CurrentMassGainRate) * (UIManager.instance.NormalizedUserInputRatioRegular / totInput);
                    CurrentMassAnti += Time.deltaTime * (UIManager.instance.NormalizedUserInputMassGainRatio * CurrentMassGainRate) * (UIManager.instance.NormalizedUserInputRatioAnti / totInput);
                    CurrentMassDark += Time.deltaTime * (UIManager.instance.NormalizedUserInputMassGainRatio * CurrentMassGainRate) * (UIManager.instance.NormalizedUserInputRatioDark / totInput);
                }
                
                // Call UI Manager to update values
                UIManager.instance.UpdateUI();

                if(phase.MissionTime < 0 && !phaseFailed) {
                    PhaseIsOnGoing = false;
                    startSound.Play(startAudioSource);

                    UIManager.instance.UserInputRatioRegular = 0;
                    UIManager.instance.UserInputRatioAntiButton1 = false;
                    UIManager.instance.UserInputRatioAntiButton2 = false;
                    UIManager.instance.UserInputRatioAntiButton3 = false;
                    UIManager.instance.UserInputRatioDark = 0;
                    UIManager.instance.UserInputMassGainRatio = 0;

                    UIManager.instance.UpdatePanelText("\n> phase completed", 1f, false, false);
                    yield return new WaitForSeconds(1.5f);
                    UIManager.instance.UpdatePanelText("\n> calculating phase results", 1f, false, false);
                    yield return new WaitForSeconds(1.5f);
                    UIManager.instance.UpdatePanelText("\nLOADING [............]", 3f, false, false);
                    yield return new WaitForSeconds(3.5f);
                    StartCoroutine(UIManager.instance.UpdatePanelForResults());
                    yield return new WaitForSeconds(12f);
                }

                if(UIManager.instance.UserInputExplode || CurrentVolatility > 0.95f) {
                    PhaseIsOnGoing = true;
                    if(!UIManager.instance.UserInputExplode)
                        UIManager.instance.UserInputExplode = true;
                    PhaseIsOnGoing = false;
                    phaseFailed = true;
                    UIManager.instance.UpdatePanelText($"> singularity collapse", 1.5f, false, true);
                    yield return new WaitForSeconds(2f);
                    UIManager.instance.UpdatePanelText($"\n> good job...", 2f, false, false);
                    yield return new WaitForSeconds(2.5f);
                    UIManager.instance.UpdatePanelText($" *sarcastic*", 0.5f, false, false);
                    yield return new WaitForSeconds(1f);
                    UIManager.instance.UpdatePanelText($"\n> rebooting....", 3f, false, false);
                    yield return new WaitForSeconds(3.5f);
                    UIManager.instance.UserInputExplode = false;
                    StartCoroutine(GameManager.instance.StartAnimation());
                }
            }

            yield return null;
        }

        if(!phaseFailed) {
            yield return new WaitForSeconds(2f);

            if(phaseQueueData.Count > 0) {
                CurrentPhase = GenerateNewPhase(phaseQueueData.Dequeue());

                // Start the mission
                StartCoroutine(PhaseLoop(CurrentPhase));
            }
            else {
                UIManager.instance.UpdatePanelText("> big bang singularity fully charged", 2f, false, true);
                yield return new WaitForSeconds(2.5f);

                UIManager.instance.UpdatePanelText("\n> awaiting explosion input...", 2f, false, false);
                PhaseIsOnGoing = true;
                IsGameFinishedAndWaitingForUserToExplode = true;
                while(!UIManager.instance.UserInputExplode) {
                    yield return null;
                }
                UIManager.instance.UserInputExplode = false;
                IsGameFinishedAndWaitingForUserToExplode = false;
                PhaseIsOnGoing = false;
                yield return new WaitForSeconds(1f);
                UIManager.instance.UpdatePanelText("\n> big bang created", 2f, false, false);
                yield return new WaitForSeconds(2.5f);
                UIManager.instance.UpdatePanelText("\n> congratz", 2f, false, false);

                yield return new WaitForSeconds(4f);

                UIManager.instance.UpdatePanelText("> start again?", 1f, false, true);

                yield return new WaitForSeconds(1.5f);
                UIManager.instance.UpdatePanelText("\n> yes", 1f, true, false);

                yield return new WaitForSeconds(4f);
                StartNewBigBang();
            }
        }
    }
}
