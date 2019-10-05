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

    // For easy access
    public Phase CurrentPhase {
        get { return BangManager.instance.CurrentPhase;
        }
    }


    // User Input Values
    public float UserInputRatioRegular { get; set; }
    public float UserInputRatioDark { get; set; }
    public bool UserInputRatioAntiButton1 { get; set; }
    public bool UserInputRatioAntiButton2 { get; set; }
    public bool UserInputRatioAntiButton3 { get; set; }
    public float UserInputMassGainRatio { get; set; }

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

    public bool UserInputPause { get; set; }
    public bool UserInputExplodeCover { get; set; }
    public bool UserInputExplode { get; set; }

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
    }

    public void UpdateUI() {
        if(CurrentPhase != null) {
            missionText.text = $"> mission {CurrentPhase.MissionText} \n> target mass {CurrentPhase.TargetMass.ToString("F")} solar masses\n> proportions <color=#{ColorUtility.ToHtmlStringRGBA(colorMassRegular)}>{(CurrentPhase.MassRatioRegular * 100f).ToString("F")}</color>" 
                + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassDark)}>{(CurrentPhase.MassRatioDark * 100f).ToString("F")}</color>"
                + $" / <color=#{ColorUtility.ToHtmlStringRGBA(colorMassAnti)}>{(CurrentPhase.MassRatioAnti * 100f).ToString("F")}</color>";
            missionTimer.text = new DateTime().AddSeconds(Mathf.Clamp(Mathf.CeilToInt(CurrentPhase.MissionTime), 0, float.MaxValue)).ToString("mm:ss");

            gaugeDensity.localEulerAngles = Vector3.Lerp(gaugeDensity.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentDensity * 270f) + 359f), 0.02f);
            gaugeTemperature.localEulerAngles = Vector3.Lerp(gaugeTemperature.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentTemperature * 270f) + 359f), 0.02f);
            gaugeVolatility.localEulerAngles = Vector3.Lerp(gaugeVolatility.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentVolatility * 270f) + 359f), 0.02f);
            gaugeMass.localEulerAngles = Vector3.Lerp(gaugeMass.localEulerAngles, new Vector3(0f, 0f, (-BangManager.instance.CurrentMass / CurrentPhase.TargetMass * 225f) + 359f), 0.02f);
            gaugeMassRate.localEulerAngles = Vector3.Lerp(gaugeMassRate.localEulerAngles, new Vector3(0f, 0f, (-NormalizedUserInputMassGainRatio * 270f) + 359f), 0.02f);
        }
    }
}
