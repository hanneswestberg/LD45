public class Phase
{
    public Phase() {}

    // Targets
    public float TargetMass { get; set; }
    public float MassRatioRegular { get; set; }
    public float MassRatioDark { get; set; }
    public float MassRatioAnti { get; set; }

    // Other
    public RangedFloat AcceptedErrorMargin { get; set; }
    public float ErrorPenalty { get; set; }
    public string MissionText { get; set; }
    public string PhaseName { get; set; }
    public float MissionTime { get; set; }
}
