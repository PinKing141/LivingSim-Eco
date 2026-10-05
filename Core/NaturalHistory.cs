namespace LivingSim.Core;

/// <summary>Filtered world milestones suitable for a history timeline.</summary>
public sealed class NaturalHistory
{
    private static readonly AnimalSpecies[] AllSpecies = Enum.GetValues<AnimalSpecies>();
    private readonly List<NaturalHistoryEvent> _events = [];
    private readonly Dictionary<AnimalSpecies, int> _previousCounts = [];
    private readonly Dictionary<AnimalSpecies, int> _highWaterMarks = [];
    private readonly Dictionary<int, bool> _groupWasMigrating = [];
    private readonly Dictionary<string, long> _lastEventTicks = [];
    private double? _baselineHerbivoreSpeed;
    private double? _baselinePredatorSpeed;

    public IReadOnlyList<NaturalHistoryEvent> Events => _events;

    internal NaturalHistorySnapshot Snapshot() => new(
        _events.ToArray(),
        new Dictionary<AnimalSpecies, int>(_previousCounts),
        new Dictionary<AnimalSpecies, int>(_highWaterMarks),
        new Dictionary<int, bool>(_groupWasMigrating),
        new Dictionary<string, long>(_lastEventTicks),
        _baselineHerbivoreSpeed,
        _baselinePredatorSpeed);

    internal void Restore(NaturalHistorySnapshot snapshot)
    {
        _events.Clear();
        _events.AddRange(snapshot.Events);
        RestoreDictionary(_previousCounts, snapshot.PreviousCounts);
        RestoreDictionary(_highWaterMarks, snapshot.HighWaterMarks);
        RestoreDictionary(_groupWasMigrating, snapshot.GroupWasMigrating);
        RestoreDictionary(_lastEventTicks, snapshot.LastEventTicks);
        _baselineHerbivoreSpeed = snapshot.BaselineHerbivoreSpeed;
        _baselinePredatorSpeed = snapshot.BaselinePredatorSpeed;
    }

    public void Update(WorldSimulation simulation)
    {
        foreach (var species in AllSpecies)
        {
            var count = simulation.PopulationBySpecies.TryGetValue(species, out var value) ? value : 0;
            var previous = _previousCounts.GetValueOrDefault(species);
            var highWater = _highWaterMarks.GetValueOrDefault(species);
            if (previous > 0 && count == 0) Add(simulation, NaturalHistoryEventType.Extinction, species, null, $"{species} went extinct.");
            else if (count > highWater && count >= highWater + Math.Max(3, highWater / 4))
                Add(simulation, NaturalHistoryEventType.PopulationHigh, species, null, $"{species} population reached {count}.");
            else if (previous >= 4 && count <= previous / 2)
                Add(simulation, NaturalHistoryEventType.PopulationLow, species, null, $"{species} population fell from {previous} to {count}.");
            _previousCounts[species] = count;
            _highWaterMarks[species] = Math.Max(highWater, count);
        }

        foreach (var group in simulation.Groups.Values)
        {
            var wasMigrating = _groupWasMigrating.GetValueOrDefault(group.GroupId);
            if (group.IsMigrating && !wasMigrating)
                Add(simulation, NaturalHistoryEventType.Migration, group.Species, group.GroupId, $"{group.Species} group {group.GroupId} began migrating.", group.MigrationTargetX, group.MigrationTargetY);
            _groupWasMigrating[group.GroupId] = group.IsMigrating;
        }

        TrackTrait(simulation, ref _baselineHerbivoreSpeed, simulation.Metrics.HerbivoreTraits.Speed, "plant-eater");
        TrackTrait(simulation, ref _baselinePredatorSpeed, simulation.Metrics.PredatorTraits.Speed, "predator");
    }

    private void TrackTrait(WorldSimulation simulation, ref double? baseline, double current, string label)
    {
        if (!baseline.HasValue) { baseline = current; return; }
        if (current == 0 || Math.Abs(current - baseline.Value) < 1) return;
        Add(simulation, NaturalHistoryEventType.TraitMilestone, null, null, $"Mean {label} speed shifted from {baseline.Value:F1} to {current:F1}.");
        baseline = current;
    }

    private void Add(WorldSimulation simulation, NaturalHistoryEventType type, AnimalSpecies? species, int? groupId, string summary, int? x = null, int? y = null)
    {
        // Migration may begin in many local groups on the same tick; retain the representative
        // population-level event rather than flooding history with equivalent notices.
        var key = type == NaturalHistoryEventType.Migration
            ? $"{type}:{species}"
            : $"{type}:{species}:{groupId}";
        var minimumInterval = type switch
        {
            NaturalHistoryEventType.TraitMilestone => 1_000,
            NaturalHistoryEventType.Migration => 500,
            NaturalHistoryEventType.PopulationHigh or NaturalHistoryEventType.PopulationLow => 250,
            _ => 0,
        };
        if (_lastEventTicks.TryGetValue(key, out var lastTick) && simulation.Tick - lastTick < minimumInterval) return;

        var location = FirstLivingLocation(simulation, species);
        _events.Add(new NaturalHistoryEvent(simulation.Tick, type, species, groupId, x ?? location.X, y ?? location.Y, summary));
        _lastEventTicks[key] = simulation.Tick;
    }

    private static (int X, int Y) FirstLivingLocation(WorldSimulation simulation, AnimalSpecies? species)
    {
        foreach (var animal in simulation.Entities.Items)
            if (animal.IsAlive && (!species.HasValue || animal.Species == species)) return (animal.X, animal.Y);
        return (simulation.World.Width / 2, simulation.World.Height / 2);
    }

    private static void RestoreDictionary<TKey, TValue>(Dictionary<TKey, TValue> target, IReadOnlyDictionary<TKey, TValue> source) where TKey : notnull
    {
        target.Clear();
        foreach (var (key, value) in source) target.Add(key, value);
    }
}
