namespace LivingSim.Core;

/// <summary>Portable, versioned representation of a runnable vivarium.</summary>
public sealed class SimulationSave
{
    public int FormatVersion { get; init; } = 2;
    public required string SimulationVersion { get; init; }
    public required DateTimeOffset CreatedUtc { get; init; }
    public required WorldSettings Settings { get; init; }
    public long Tick { get; init; }
    public ClimateState Climate { get; init; }
    public ClimateMode ClimateMode { get; init; }
    public required WorldCell[] Cells { get; init; }
    public required AnimalState[] Animals { get; init; }
    public required Carcass[] Carcasses { get; init; }
    public required ClimateRecord[] ClimateHistory { get; init; }
    public required LineageRecord[] LineageRecords { get; init; }
    public required NaturalHistorySnapshot NaturalHistory { get; init; }
}
