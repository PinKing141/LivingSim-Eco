namespace LivingSim.Core;

public enum SimulationSystem : byte
{
    Climate,
    ResourceRegeneration,
    Metabolism,
    SpatialIndexUpdate,
    Perception,
    Movement,
    Combat,
    Feeding,
    LifecycleAndDeath,
    CarcassProcessing,
    Metrics,
}
