namespace LivingSim.Core;

public static class ClimateModel
{
    public const int TicksPerSeason = 120;
    public const int ClimatePeriodTicks = 8_000;

    public static ClimateState At(long tick)
    {
        var season = (Season)((tick / TicksPerSeason) % 4);
        var phase = (int)(tick % ClimatePeriodTicks);
        var halfPeriod = ClimatePeriodTicks / 2;
        var climateIndex = phase <= halfPeriod
            ? -1_000 + phase * 2_000 / halfPeriod
            : 1_000 - (phase - halfPeriod) * 2_000 / halfPeriod;
        var seasonalModifier = season switch
        {
            Season.Spring => 1_300,
            Season.Summer => 1_100,
            Season.Autumn => 900,
            Season.Winter => 500,
            _ => throw new ArgumentOutOfRangeException(),
        };
        return new ClimateState(tick, season, climateIndex, seasonalModifier * (1_000 + climateIndex / 2) / 1_000);
    }
}
