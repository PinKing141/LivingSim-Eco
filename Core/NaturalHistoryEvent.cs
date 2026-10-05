namespace LivingSim.Core;

public enum NaturalHistoryEventType
{
    PopulationHigh,
    PopulationLow,
    Extinction,
    Migration,
    TraitMilestone,
}

public readonly record struct NaturalHistoryEvent(
    long Tick,
    NaturalHistoryEventType Type,
    AnimalSpecies? Species,
    int? GroupId,
    int X,
    int Y,
    string Summary);
