namespace LivingSim.Core;

public sealed class ClimateHistory
{
    private readonly List<ClimateRecord> _records = [];

    public IReadOnlyList<ClimateRecord> Records => _records;

    internal void Record(long tick, ClimateState climate, TraitAverages plantEaterTraits, TraitAverages hunterTraits)
    {
        if (_records.Count == 0 || tick - _records[^1].Tick >= ClimateModel.TicksPerSeason)
        {
            _records.Add(new ClimateRecord(tick, climate, plantEaterTraits, hunterTraits));
        }
    }

    internal void Restore(IEnumerable<ClimateRecord> records)
    {
        _records.Clear();
        _records.AddRange(records.OrderBy(record => record.Tick));
    }
}
