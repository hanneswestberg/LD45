using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PhaseData")]
public class PhaseData : ScriptableObject
{
#pragma warning disable 0649
    [Header("Phase Data:")]
    [SerializeField]
    private string phaseName;


    [SerializeField]
    [MinMaxRange(0, 1)]
    private RangedFloat percentDifferances;

    [Space]
    [SerializeField]
    [MinMaxRange(1, 20)]
    private RangedFloat newMassFactor;

    [Space]
    [SerializeField]
    [MinMaxRange(0, 1)]
    private RangedFloat acceptedErrorMargin;

    [Space]
    [SerializeField]
    [MinMaxRange(0, 1)]
    private RangedFloat errorPenalty;

    [Space]
    [SerializeField]
    [MinMaxRange(0, 120)]
    private RangedFloat missionTime;

    [Space]
    [SerializeField]
    [TextArea]
    private string missionText;

    public string PhaseName { get { return phaseName; } }
    public RangedFloat PercentDifferances { get { return percentDifferances; } }
    public RangedFloat NewMassFactor { get { return newMassFactor; } }
    public RangedFloat AcceptedErrorMargin { get { return acceptedErrorMargin; } }
    public RangedFloat ErrorPenalty { get { return errorPenalty; } }
    public RangedFloat MissionTime { get { return missionTime; } }
    public string MissionText { get { return missionText; } }
}
