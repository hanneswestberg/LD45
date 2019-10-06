using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

#pragma warning disable 0649
    [Header("UI Colors:")]
    [SerializeField]
    private Color colorMassRegular;
    [SerializeField]
    private Color colorMassAnti;
    [SerializeField]
    private Color colorMassDark;

    [Header("Non interactable:")]
    [SerializeField]
    private RectTransform gaugeDensity;
    [SerializeField]
    private RectTransform gaugeTemperature;
    [SerializeField]
    private RectTransform gaugeVolatility;
    [SerializeField]
    private RectTransform gaugeMass;
    [SerializeField]
    private RectTransform gaugeMassRate;
    [SerializeField]
    private TextMeshProUGUI missionText;
    [SerializeField]
    private TextMeshProUGUI missionTimer;
    [SerializeField]
    private List<Image> phaseLights;

    [Header("Interactable:")]
    [SerializeField]
    private Slider sliderMassGainRatio;
    [SerializeField]
    private RectTransform imageSwitchPause;
    [SerializeField]
    private RectTransform imageSwitchExplodeCover;
    [SerializeField]
    private Button buttonExplode;

    [Header("Sounds:")]
    [SerializeField]
    private SimpleAudioEvent timerSound;
    [SerializeField]
    private SimpleAudioEvent timerLowSound;
    [SerializeField]
    private SimpleAudioEvent keyboardSound;
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

    [Header("AudioSources:")]
    [SerializeField]
    private AudioSource timerAudioSource;
    [SerializeField]
    private AudioSource keyboardAudioSource;
    [SerializeField]
    private AudioSource matterFlowAudioSource;
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
    private AudioSource explodeCoverAudioSource;

    // For easy access
    public Phase CurrentPhase {
        get { return BangManager.instance.CurrentPhase;
        }
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
    public bool UserInputExplode { get; set; }

    private int tempTimer = 0;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        imageSwitchPause.localScale = new Vector3(1f, (UserInputPause ? -1f : 1f), 1f);
        imageSwitchPause.localPosition = new Vector3(0f, (UserInputPause ? -10f : 10f), 0f);

        imageSwitchExplodeCover.localScale = new Vector3(1f, (UserInputExplodeCover ? -1f : 1f), 1f);
        buttonExplode.interactable = (UserInputExplodeCover);

        if(CurrentPhase != null) {
            matterFlowAudioSource.volume = NormalizedUserInputMassGainRatio / 10f;
            matterFlowAudioSource.pitch = 0.75f + NormalizedUserInputMassGainRatio/2f;
        }
    }

    public void UpdateUI() {
        if(CurrentPhase != null) {
            // Timer:
            int timerValue = Mathf.Clamp(Mathf.CeilToInt(CurrentPhase.MissionTime), 0, int.MaxValue);
            missionTimer.text = new DateTime().AddSeconds(timerValue).ToString("mm:ss");
            if(timerValue < tempTimer) {
                if(timerValue > 10)
                    timerSound.Play(timerAudioSource);
                else
                    timerLowSound.Play(timerAudioSource);
            }
            tempTimer = timerValue;



            gaugeDensity.localEulerAngles = Vector3.Lerp(gaugeDensity.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentDensity * 270f) + 359f), 0.02f);
            gaugeTemperature.localEulerAngles = Vector3.Lerp(gaugeTemperature.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentTemperature * 270f) + 359f), 0.02f);
            gaugeVolatility.localEulerAngles = Vector3.Lerp(gaugeVolatility.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentVolatility * 270f) + 359f), 0.02f);
            gaugeMass.localEulerAngles = Vector3.Lerp(gaugeMass.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentMass / CurrentPhase.TargetMass * 225f) + 359f), 0.02f);
            gaugeMassRate.localEulerAngles = Vector3.Lerp(gaugeMassRate.localEulerAngles, new Vector3(0f, 0f, (-NormalizedUserInputMassGainRatio * 270f) + 359f), 0.02f);
        }
    }

    public void UpdatePanelForMission() {

        var text = $"> target mass {CurrentPhase.TargetMass.ToString("F")} solar masses\n> proportions <color=#{ColorUtility.ToHtmlStringRGBA(colorMassRegular)}>{(CurrentPhase.MassRatioRegular * 100f).ToString("F")}</color>"
    + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassDark)}>{(CurrentPhase.MassRatioDark * 100f).ToString("F")}</color>"
    + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassAnti)}>{(CurrentPhase.MassRatioAnti * 100f).ToString("F")}</color>";

        UpdatePanelText(text, 3f, false, true);
    }

    public void UpdatePanelText(string text, float time, bool humanInput, bool clearInput = false) {
        if(clearInput)
            missionText.text = "";

        StartCoroutine(UpdateMissionTextIEnumerator(text, time, humanInput));
    }

    private IEnumerator UpdateMissionTextIEnumerator(string text, float time, bool humanInput) {

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

        yield return null;
    }
}
