namespace LivingSim.Core;

public readonly record struct ClimateState(long Tick, Season Season, int ClimateIndexMilli, int BiomassModifierMilli)
{
    public bool IsDrought => ClimateIndexMilli <= -500;
    public bool IsAbundance => ClimateIndexMilli >= 500;
}
