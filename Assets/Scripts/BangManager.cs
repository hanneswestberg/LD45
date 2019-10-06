﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    private float currenDensity = 0;
    public float CurrentDensity {
        get {
            return Mathf.Clamp(Mathf.Lerp(currenDensity, currenDensity + (UIManager.instance.NormalizedUserInputMassGainRatio - 0.5f), 1f), 0f, 1f);
        }
    }
    private float currenTemperature = 0;
    public float CurrentTemperature {
        get {
            return Mathf.Clamp(Mathf.Lerp(currenTemperature, currenTemperature + (UIManager.instance.NormalizedUserInputMassGainRatio - 0.5f), 1f), 0f, 1f);
        }
    }
    public float CurrentVolatility { get; private set; }

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


    public void StartNewBigBang() {
        // Start everything here
        CurrentMassRegular = 0.01f;
        CurrentMassDark = 0.01f;
        CurrentMassAnti = 0.01f;
        CurrentVolatility = 0.01f;

        // Don't start if we have no phases set
        if(!phaseData.Any())
            return;

        phaseQueueData = new Queue<PhaseData>();
        foreach(var phase in phaseData) {
            phaseQueueData.Enqueue(phase);
        }

        // Generate the phase
        CurrentPhase = GenerateNewPhase(phaseQueueData.Dequeue(), 1f);

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

    /// <summary>
    /// Generate cool new phases
    /// </summary>
    /// <param name="phaseData"></param>
    /// <returns>A new phase</returns>
    private Phase GenerateNewPhase(PhaseData phaseData, float overrideCurrentMass = 0) {
        var model = new  Phase() {
            TargetMass = (overrideCurrentMass != 0 ? overrideCurrentMass : CurrentMass) * Random.Range(phaseData.NewMassFactor.minValue, phaseData.NewMassFactor.maxValue),
            AcceptedErrorMargin = phaseData.AcceptedErrorMargin,
            ErrorPenalty = Random.Range(phaseData.AcceptedErrorMargin.minValue, phaseData.AcceptedErrorMargin.maxValue),
            MissionText = phaseData.MissionText,
            MissionTime = Random.Range(phaseData.MissionTime.minValue, phaseData.MissionTime.maxValue)
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

        Debug.Log($"Target Mass: {model.TargetMass}, Regular: {model.MassRatioRegular}, Anti: {model.MassRatioAnti}, Dark: {model.MassRatioDark}, Total: {model.MassRatioRegular + model.MassRatioAnti + model.MassRatioDark}");
        return model;
    }

    /// <summary>
    /// The main phase loop
    /// </summary>
    /// <param name="phase"></param>
    /// <returns></returns>
    private IEnumerator PhaseLoop(Phase phase) {

        UIManager.instance.UpdatePanelText($"> start {phase.MissionText}", 2f, true, true);
        yield return new WaitForSeconds(3f);
        UIManager.instance.UpdatePanelText("\n...", 1f, false, false);
        startSound.Play(startAudioSource);

        yield return new WaitForSeconds(2f);
        phaseSound.Play(phaseAudioSource);
        UIManager.instance.UpdateUI();
        UIManager.instance.UpdatePanelForMission();

        yield return new WaitForSeconds(2f);

        // Wait and show message here

        var phaseIsOnGoing = true;
        while(phaseIsOnGoing) {

            if(!UIManager.instance.UserInputPause) {
                // Update mission timer
                phase.MissionTime -= Time.deltaTime;

                // Normalize input values
                var totInput = (UIManager.instance.NormalizedUserInputRatioRegular + UIManager.instance.NormalizedUserInputRatioDark + UIManager.instance.NormalizedUserInputRatioAnti);

                if(totInput > 0) {
                    CurrentMassRegular += Time.deltaTime * (UIManager.instance.NormalizedUserInputMassGainRatio * CurrentMassGainRate) * (UIManager.instance.NormalizedUserInputRatioRegular / totInput);
                    CurrentMassAnti += Time.deltaTime * (UIManager.instance.NormalizedUserInputMassGainRatio * CurrentMassGainRate) * (UIManager.instance.NormalizedUserInputRatioAnti / totInput);
                    CurrentMassDark += Time.deltaTime * (UIManager.instance.NormalizedUserInputMassGainRatio * CurrentMassGainRate) * (UIManager.instance.NormalizedUserInputRatioDark / totInput);
                }

                if(phase.MissionTime < 0) {
                    phaseIsOnGoing = false;

                    UIManager.instance.UserInputRatioRegular = 0;
                    UIManager.instance.UserInputRatioAntiButton1 = false;
                    UIManager.instance.UserInputRatioAntiButton2 = false;
                    UIManager.instance.UserInputRatioAntiButton3 = false;
                    UIManager.instance.UserInputRatioDark = 0;
                    UIManager.instance.UserInputMassGainRatio = 0;
                }

                // Call UI Manager to update values
                UIManager.instance.UpdateUI();
            }

            yield return null;
        }

        startSound.Play(phaseAudioSource);
        yield return new WaitForSeconds(2f);

        CurrentPhase = GenerateNewPhase(phaseQueueData.Dequeue());

        // Start the mission
        StartCoroutine(PhaseLoop(CurrentPhase));
    }
}
