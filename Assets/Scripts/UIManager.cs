using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private BangManager bgMan;

#pragma warning disable 0649
    [Header("UI Colors:")]
    [SerializeField]
    private Color colorMassRegular;
    [SerializeField]
    private Color colorMassAnti;
    [SerializeField]
    private Color colorMassDark;
    [SerializeField]
    private Color colorTargetMass;
    [SerializeField]
    private Color colorDiodeOff;
    [SerializeField]
    private Color colorDiodeOn;
    [SerializeField]
    private Color colorDiodeSuccess;
    [SerializeField]
    private Color colorDiodeFail;
    [SerializeField]
    private Color colorVolatility;

    [Header("Non interactable:")]
    [SerializeField]
    private RectTransform gaugeDensity;
    [SerializeField]
    private RectTransform gaugeTemperature;
    [SerializeField]
    private RectTransform gaugeVolatility;
    [SerializeField]
    private TextMeshProUGUI currentMassText;
    [SerializeField]
    private RectTransform gaugeMassRate;
    [SerializeField]
    private TextMeshProUGUI missionText;
    [SerializeField]
    private TextMeshProUGUI missionTimer;
    [SerializeField]
    private List<Image> phaseLights;
    [SerializeField]
    private Image currentMassLightSuccess;
    [SerializeField]
    private Image currentMassLightWarning;
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private Image temperatureLight;
    [SerializeField]
    private Image densityLight;

    [Header("Interactable:")]
    [SerializeField]
    private Slider sliderMassGainRatio;
    [SerializeField]
    private RectTransform imageSwitchPause;
    [SerializeField]
    private RectTransform imageSwitchExplodeCover;
    [SerializeField]
    private Button buttonExplode;
    [SerializeField]
    private GameObject note;

    [Header("Sounds:")]
    [SerializeField]
    private SimpleAudioEvent timerSound;
    [SerializeField]
    private SimpleAudioEvent timerLowSound;
    [SerializeField]
    private SimpleAudioEvent keyboardSound;
    [SerializeField]
    private SimpleAudioEvent keyboardEnterSound;
    [SerializeField]
    private SimpleAudioEvent switchSound;
    [SerializeField]
    private SimpleAudioEvent buttonSound;
    [SerializeField]
    private SimpleAudioEvent megaSwitchSound;
    [SerializeField]
    private SimpleAudioEvent explodeCoverSound;
    [SerializeField]
    private SimpleAudioEvent computerNoise;
    [SerializeField]
    private SimpleAudioEvent diodeSuccess;
    [SerializeField]
    private SimpleAudioEvent diodeFail;
    [SerializeField]
    private SimpleAudioEvent explodeSound;
    [SerializeField]
    private SimpleAudioEvent computerFormattedNoise;
    [SerializeField]
    private SimpleAudioEvent alarmSound;
    [SerializeField]
    private SimpleAudioEvent slowTimeSound;
    [SerializeField]
    private SimpleAudioEvent fastTimeSound;
    [SerializeField]
    private SimpleAudioEvent paperSound;

    [Header("AudioSources:")]
    [SerializeField]
    private AudioSource timerAudioSource;
    [SerializeField]
    private AudioSource keyboardAudioSource;
    [SerializeField]
    private AudioSource matterFlowRegularAudioSource;
    [SerializeField]
    private AudioSource matterFlowAntiAudioSource;
    [SerializeField]
    private AudioSource matterFlowDarkAudioSource;
    [SerializeField]
    private AudioSource ratioRegularAudioSource;
    [SerializeField]
    private AudioSource ratioDarkAudioSource;
    [SerializeField]
    private AudioSource ratioAntiAudioSource;
    [SerializeField]
    private AudioSource ratioMassAudioSource;
    [SerializeField]
    private AudioSource pauseAudioSource;
    [SerializeField]
    private AudioSource pauseEffectsAudioSource;
    [SerializeField]
    private AudioSource explodeCoverAudioSource;
    [SerializeField]
    private AudioSource massDiodeAudioSource;
    [SerializeField]
    private AudioSource explosionAudioSource;
    [SerializeField]
    private AudioSource temperatureAudioSource;
    [SerializeField]
    private AudioSource densityAudioSource;

    // For easy access
    public Phase CurrentPhase {
        get { return bgMan.CurrentPhase;
        }
    }

    public void NoteComplete()
    {
        note.SetActive(false);
        paperSound.Play(keyboardAudioSource);
        StartCoroutine(GameManager.instance.StartAnimation());
    }

    // User Input Values
    private float userInputRatioRegular;
    public float UserInputRatioRegular {
        get {
            return userInputRatioRegular;
        }
        set {
            switchSound.Play(ratioRegularAudioSource);
            userInputRatioRegular = value;
        }
    }

    private float userInputRatioDark;
    public float UserInputRatioDark {
        get {
            return userInputRatioDark;
        }
        set {
            switchSound.Play(ratioDarkAudioSource);
            userInputRatioDark = value;
        }
    }

    private bool userInputRatioAntiButton1;
    public bool UserInputRatioAntiButton1 {
        get {
            return userInputRatioAntiButton1;
        }
        set {
            buttonSound.Play(ratioAntiAudioSource);
            userInputRatioAntiButton1 = value;
        }
    }
    private bool userInputRatioAntiButton2;
    public bool UserInputRatioAntiButton2 {
        get {
            return userInputRatioAntiButton2;
        }
        set {
            buttonSound.Play(ratioAntiAudioSource);
            userInputRatioAntiButton2 = value;
        }
    }

    private bool userInputRatioAntiButton3;
    public bool UserInputRatioAntiButton3 {
        get {
            return userInputRatioAntiButton3;
        }
        set {
            buttonSound.Play(ratioAntiAudioSource);
            userInputRatioAntiButton3 = value;
        }
    }

    private float userInputMassGainRatio;
    public float UserInputMassGainRatio {
        get {
            return userInputMassGainRatio;
        }
        set {
            switchSound.Play(ratioMassAudioSource);
            userInputMassGainRatio = value;
        }
    }

    public float NormalizedUserInputRatioRegular {
        get {
            return UserInputRatioRegular / 10f;
        }
    }
    public float NormalizedUserInputRatioDark {
        get {
            return UserInputRatioDark / 10f;
        }
    }
    public float NormalizedUserInputRatioAnti {
        get {
            var value = 0f;
            value += (UserInputRatioAntiButton1) ? 1f : 0f;
            value += (UserInputRatioAntiButton2) ? 2f : 0f;
            value += (UserInputRatioAntiButton3) ? 3f : 0f;
            return value/6f;
        }
    }
    public float NormalizedUserInputMassGainRatio {
        get {
            return UserInputMassGainRatio / 10f;
        }
    }

    private bool userInputPause;
    public bool UserInputPause {
        get {
            return userInputPause;
        }
        set {
            megaSwitchSound.Play(pauseAudioSource);
            userInputPause = value;
            if(value == true) {
                slowTimeSound.Play(pauseEffectsAudioSource);
            }
            else {
                fastTimeSound.Play(pauseEffectsAudioSource);
            }
        }
    }

    private bool userInputExplodeCover;
    public bool UserInputExplodeCover {
        get {
            return userInputExplodeCover;
        }
        set {
            explodeCoverSound.Play(explodeCoverAudioSource);
            userInputExplodeCover = value;
        }
    }

    private bool userInputExplode;
    public bool UserInputExplode {
        get
        {
            return userInputExplode;
        }
        set
        {
            if(value == true && bgMan.PhaseIsOnGoing) {
                explodeSound.Play(explosionAudioSource);
                explosion.SetActive(true);
                userInputExplode = true;
            }
            else {
                userInputExplode = false;
            }
        }
    }

    private int tempTimer = 0;
    private Color tempMassDiodeColorSuccess;
    private Color tempMassDiodeColorFail;
    private Color tempTemperatureColor;
    private Color tempDensityColor;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        bgMan = BangManager.instance;
    }

    private void Update() {
        imageSwitchPause.localScale = new Vector3(1f, (UserInputPause ? -1f : 1f), 1f);
        imageSwitchPause.localPosition = new Vector3(0f, (UserInputPause ? -10f : 10f), 0f);

        imageSwitchExplodeCover.localScale = new Vector3(1f, (UserInputExplodeCover ? -1f : 1f), 1f);
        buttonExplode.interactable = (UserInputExplodeCover);

        if(CurrentPhase != null) {
            matterFlowRegularAudioSource.volume = (NormalizedUserInputRatioRegular * NormalizedUserInputMassGainRatio) / 12f;
            matterFlowRegularAudioSource.pitch = (!UserInputPause) ? 0.75f + (NormalizedUserInputRatioRegular * NormalizedUserInputMassGainRatio) / 2f : Mathf.Lerp(matterFlowRegularAudioSource.pitch, 0f, 0.05f);

            matterFlowAntiAudioSource.volume = (NormalizedUserInputRatioAnti * NormalizedUserInputMassGainRatio) / 8f;
            matterFlowAntiAudioSource.pitch = (!UserInputPause) ? 0.75f + (NormalizedUserInputRatioAnti * NormalizedUserInputMassGainRatio) / 2f : Mathf.Lerp(matterFlowAntiAudioSource.pitch, 0f, 0.05f);

            matterFlowDarkAudioSource.volume = (NormalizedUserInputRatioDark * NormalizedUserInputMassGainRatio) / 8f;
            matterFlowDarkAudioSource.pitch = (!UserInputPause) ? 0.75f + (NormalizedUserInputRatioDark * NormalizedUserInputMassGainRatio) / 2f : Mathf.Lerp(matterFlowDarkAudioSource.pitch, 0f, 0.05f);
        }
    }

    public void UpdateUI() {
        if(CurrentPhase != null) {
            // Timer:
            int timerValue = Mathf.Clamp(Mathf.CeilToInt(CurrentPhase.MissionTime), 0, int.MaxValue);
            missionTimer.text = new DateTime().AddSeconds(timerValue).ToString("mm:ss");
            missionTimer.color = (timerValue <= 10f) ? colorDiodeFail : colorDiodeOn;
            if(timerValue < tempTimer) {
                if(timerValue > 10)
                    timerSound.Play(timerAudioSource);
                else
                    timerLowSound.Play(timerAudioSource);
            }
            tempTimer = timerValue;

            gaugeDensity.localEulerAngles = Vector3.Lerp(gaugeDensity.localEulerAngles, new Vector3(0f, 0f, 359f - bgMan.CurrentDensity * 270f), 0.02f);
            gaugeTemperature.localEulerAngles = Vector3.Lerp(gaugeTemperature.localEulerAngles, new Vector3(0f, 0f, 359f - bgMan.CurrentTemperature * 270f), 0.02f);
            gaugeVolatility.localEulerAngles = Vector3.Lerp(gaugeVolatility.localEulerAngles, new Vector3(0f, 0f, 359f - bgMan.CurrentVolatility * 270f), 0.02f);
            gaugeMassRate.localEulerAngles = Vector3.Lerp(gaugeMassRate.localEulerAngles, new Vector3(0f, 0f, 359f - NormalizedUserInputMassGainRatio * 270f), 0.02f);

            // Current mass text and diodes
            currentMassText.text = bgMan.CurrentMass.ToString("n", new CultureInfo("en-US")) + " sm";
            currentMassLightSuccess.color = (bgMan.CurrentMass > CurrentPhase.TargetMass && (bgMan.CurrentMass / CurrentPhase.TargetMass) < 1.1f) ? colorDiodeSuccess : colorDiodeOff;
            currentMassLightWarning.color = (bgMan.CurrentMass < CurrentPhase.TargetMass || (bgMan.CurrentMass / CurrentPhase.TargetMass) > 1.1f) ? colorDiodeFail : colorDiodeOff;

            if(tempMassDiodeColorSuccess != currentMassLightSuccess.color && currentMassLightSuccess.color == colorDiodeSuccess) {
                diodeSuccess.Play(massDiodeAudioSource);
            }
            if(tempMassDiodeColorFail != currentMassLightWarning.color && currentMassLightWarning.color == colorDiodeFail) {
                diodeFail.Play(massDiodeAudioSource);
            }
            tempMassDiodeColorSuccess = currentMassLightSuccess.color;
            tempMassDiodeColorFail = currentMassLightWarning.color;

            // Diodes
            foreach(var diode in phaseLights) {
                diode.color = (phaseLights.IndexOf(diode) <= bgMan.CurrentPhaseIndex) ? colorDiodeSuccess : colorDiodeOn;
            }

            temperatureLight.color = (bgMan.CurrentTemperature < 0.9f) ? colorDiodeSuccess : colorDiodeFail;
            if(tempTemperatureColor != temperatureLight.color) {
                if(temperatureLight.color == colorDiodeSuccess) {
                    diodeSuccess.Play(temperatureAudioSource);
                }
                else if(temperatureLight.color == colorDiodeFail) {
                    alarmSound.Play(temperatureAudioSource);
                }
            }
            tempTemperatureColor = temperatureLight.color;

            densityLight.color = (bgMan.CurrentDensity < 0.9f) ? colorDiodeSuccess : colorDiodeFail;
            if(tempDensityColor != densityLight.color) {
                if(densityLight.color == colorDiodeSuccess) {
                    diodeSuccess.Play(temperatureAudioSource);
                }
                else if(densityLight.color == colorDiodeFail) {
                    alarmSound.Play(temperatureAudioSource);
                }
            }
            tempDensityColor = densityLight.color;
        }
    }

    public void UpdatePanelForMission() {
        var text = $"> {CurrentPhase.PhaseName}\n> target mass ";
        var formattedText = $"<color=#{ColorUtility.ToHtmlStringRGBA(colorTargetMass)}>{CurrentPhase.TargetMass.ToString("n", new CultureInfo("en-US"))}</color> solar masses"
            + $"\n> proportions <color=#{ColorUtility.ToHtmlStringRGBA(colorMassRegular)}>{(CurrentPhase.MassRatioRegular * 100f).ToString("F1")}%</color>"
            + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassAnti)}>{(CurrentPhase.MassRatioAnti * 100f).ToString("F1")}%</color>"
            + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassDark)}>{(CurrentPhase.MassRatioDark * 100f).ToString("F1")}%</color>";

        UpdatePanelText(text, 2f, false, true, formattedText);
    }

    public IEnumerator UpdatePanelForResults() {
        UpdatePanelText($"> expected ", 0.5f, false, true, $"<color=#{ColorUtility.ToHtmlStringRGBA(colorMassRegular)}>{(CurrentPhase.MassRatioRegular * 100f).ToString("F1")}%</color>"
            + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassAnti)}>{(CurrentPhase.MassRatioAnti * 100f).ToString("F1")}%</color>"
            + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassDark)}>{(CurrentPhase.MassRatioDark * 100f).ToString("F1")}%</color>");
        yield return new WaitForSeconds(1f);

        UpdatePanelText($"\n> target mass ", 0.5f, false, false, $"<color=#{ColorUtility.ToHtmlStringRGBA(colorTargetMass)}>{CurrentPhase.TargetMass.ToString("n", new CultureInfo("en-US"))}</color> sm");
        yield return new WaitForSeconds(1f);

        UpdatePanelText($"\n> current ", 0.5f, false, false, $"<color=#{ColorUtility.ToHtmlStringRGBA(colorMassRegular)}>{(bgMan.CurrentMassRatioRegular * 100f).ToString("F1")}%</color>"
            + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassAnti)}>{(bgMan.CurrentMassRatioAnti * 100f).ToString("F1")}%</color>"
            + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassDark)}>{(bgMan.CurrentMassRatioDark * 100f).ToString("F1")}%</color>");
        yield return new WaitForSeconds(1f);

        UpdatePanelText($"\n> current mass ", 0.5f, false, false, $"<color=#{ColorUtility.ToHtmlStringRGBA(colorTargetMass)}>{bgMan.CurrentMass.ToString("n", new CultureInfo("en-US"))}</color> sm");
        yield return new WaitForSeconds(3f);

        UpdatePanelText($"\n> error margin ", 1f, false, false, $"<color=#{ColorUtility.ToHtmlStringRGBA(colorDiodeFail)}>{bgMan.CalculateErrorMargin().ToString("F1")}%</color>");
        yield return new WaitForSeconds(1.5f);

        UpdatePanelText($"\n> volatility increased by ", 1f, false, false, $"<color=#{ColorUtility.ToHtmlStringRGBA(colorVolatility)}>{bgMan.CalculateVolatilityGain().ToString("F1")}%</color>");
        yield return new WaitForSeconds(1f);

        StartCoroutine(UpdateVolatilityMeter());

        yield return new WaitForSeconds(2.5f);

        UpdateUI();
        yield return null;
    }

    public IEnumerator UpdateVolatilityMeter() {

        // Update for 3 seconds...
        var timer = 3f;
        while(timer > 0) {
            timer -= Time.deltaTime;
            gaugeVolatility.localEulerAngles = Vector3.Lerp(gaugeVolatility.localEulerAngles, new Vector3(0f, 0f, 359f - bgMan.CurrentVolatility * 270f), 0.02f);
            yield return null;
        }

    }

    public void UpdatePanelText(string text, float time, bool humanInput, bool clearInput = false, string formattedText = "") {
        if(clearInput)
            missionText.text = "";

        StartCoroutine(UpdateMissionTextIEnumerator(text, time, humanInput, formattedText));
    }

    private IEnumerator UpdateMissionTextIEnumerator(string text, float time, bool humanInput, string formatedText) {

        var interval = time / text.Length;

        foreach(var character in text) {
            yield return new WaitForSeconds(interval);
            missionText.text += character;

            if(humanInput) {
                keyboardSound.Play(keyboardAudioSource);
            }
            else {
                computerNoise.Play(keyboardAudioSource);
            }
        }
        if(humanInput) {
            keyboardEnterSound.Play(keyboardAudioSource);
        }

        if(!string.IsNullOrEmpty(formatedText)) {
            missionText.text += formatedText;
            computerFormattedNoise.Play(keyboardAudioSource);
        }

        yield return null;
    }

    private void Explode()
    {
        explosion.SetActive(true);
    }
}
