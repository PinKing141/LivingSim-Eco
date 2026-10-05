namespace LivingSim.Core;

/// <summary>Indexes births and deaths without retaining per-tick entity snapshots.</summary>
public sealed class LineageHistory
{
    private readonly Dictionary<int, LineageRecord> _records = [];
    private readonly Dictionary<int, List<int>> _children = [];

    public IReadOnlyDictionary<int, LineageRecord> Records => _records;

    public void RegisterBirth(AnimalState animal, long tick)
    {
        _records[animal.Id] = new LineageRecord(animal.Id, animal.Species, animal.ParentAId, animal.ParentBId,
            animal.Generation, tick, animal.X, animal.Y, null, animal.X, animal.Y);
        AddChild(animal.ParentAId, animal.Id);
        AddChild(animal.ParentBId, animal.Id);
    }

    public void RecordDeath(int animalId, long tick, int x, int y)
    {
        if (!_records.TryGetValue(animalId, out var record) || record.DeathTick.HasValue) return;
        _records[animalId] = record with { DeathTick = tick, DeathX = x, DeathY = y };
    }

    internal void Restore(IEnumerable<LineageRecord> records)
    {
        _records.Clear();
        _children.Clear();
        foreach (var record in records.OrderBy(record => record.AnimalId))
        {
            _records.Add(record.AnimalId, record);
            AddChild(record.ParentAId, record.AnimalId);
            AddChild(record.ParentBId, record.AnimalId);
        }
    }

    public IReadOnlyList<LineageRecord> AncestorsOf(int animalId, int maxDepth = 8) => Traverse(animalId, maxDepth, parents: true);
    public IReadOnlyList<LineageRecord> DescendantsOf(int animalId, int maxDepth = 8) => Traverse(animalId, maxDepth, parents: false);

    private void AddChild(int parentId, int childId)
    {
        if (parentId == 0) return;
        if (!_children.TryGetValue(parentId, out var children)) _children[parentId] = children = [];
        if (!children.Contains(childId)) children.Add(childId);
    }

    private IReadOnlyList<LineageRecord> Traverse(int animalId, int maxDepth, bool parents)
    {
        if (maxDepth <= 0 || !_records.ContainsKey(animalId)) return [];
        var result = new List<LineageRecord>();
        var seen = new HashSet<int> { animalId };
        var pending = new Queue<(int Id, int Depth)>();
        pending.Enqueue((animalId, 0));
        while (pending.Count > 0)
        {
            var (id, depth) = pending.Dequeue();
            if (depth >= maxDepth) continue;
            IEnumerable<int> next = parents
                ? [_records[id].ParentAId, _records[id].ParentBId]
                : _children.TryGetValue(id, out var children) ? children : [];
            foreach (var relatedId in next)
            {
                if (relatedId == 0 || !seen.Add(relatedId) || !_records.TryGetValue(relatedId, out var related)) continue;
                result.Add(related);
                pending.Enqueue((relatedId, depth + 1));
            }
        }
        return result;
    }
}
