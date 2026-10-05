namespace LivingSim.Core;

/// <summary>State needed to continue filtered natural-history detection after loading.</summary>
public sealed record NaturalHistorySnapshot(
    IReadOnlyList<NaturalHistoryEvent> Events,
    IReadOnlyDictionary<AnimalSpecies, int> PreviousCounts,
    IReadOnlyDictionary<AnimalSpecies, int> HighWaterMarks,
    IReadOnlyDictionary<int, bool> GroupWasMigrating,
    IReadOnlyDictionary<string, long> LastEventTicks,
    double? BaselineHerbivoreSpeed,
    double? BaselinePredatorSpeed);
