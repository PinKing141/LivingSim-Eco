namespace LivingSim.Observation
{
    /// <summary>
    /// Immutable metrics captured for a specific simulation tick.
    /// </summary>
    public readonly record struct MetricsSnapshot(
        long Tick,
        float TotalFood,
        float TotalWater,
        float TotalTimber);
}
