namespace LivingSim.Core;

/// <summary>Fixed buckets make local animal lookup scale independently of total world size.</summary>
public sealed class SpatialIndex
{
    private const int BucketSize = 8;
    private readonly int _bucketColumns;
    private readonly List<int>[] _buckets;

    public SpatialIndex(World world)
    {
        _bucketColumns = (world.Width + BucketSize - 1) / BucketSize;
        var rows = (world.Height + BucketSize - 1) / BucketSize;
        _buckets = new List<int>[_bucketColumns * rows];
        for (var index = 0; index < _buckets.Length; index++)
        {
            _buckets[index] = [];
        }
    }

    public void Rebuild(EntityStore entities)
    {
        foreach (var bucket in _buckets)
        {
            bucket.Clear();
        }

        foreach (var animal in entities.Items)
        {
            if (animal.IsAlive)
            {
                _buckets[BucketFor(animal.X, animal.Y)].Add(animal.Id);
            }
        }
    }

    public int FindNearestHerbivore(EntityStore entities, int x, int y, int radius)
    {
        var minimumX = Math.Max(0, (x - radius) / BucketSize);
        var maximumX = Math.Min(_bucketColumns - 1, (x + radius) / BucketSize);
        var minimumY = Math.Max(0, (y - radius) / BucketSize);
        var maximumY = Math.Min((_buckets.Length / _bucketColumns) - 1, (y + radius) / BucketSize);
        var bestId = 0;
        var bestDistance = int.MaxValue;

        for (var bucketY = minimumY; bucketY <= maximumY; bucketY++)
        {
            for (var bucketX = minimumX; bucketX <= maximumX; bucketX++)
            {
                foreach (var candidateId in _buckets[bucketY * _bucketColumns + bucketX])
                {
                    var candidate = entities.GetById(candidateId);
                    if (!candidate.IsAlive || candidate.Species != AnimalSpecies.Herbivore)
                    {
                        continue;
                    }

                    var distance = Math.Abs(candidate.X - x) + Math.Abs(candidate.Y - y);
                    if (distance <= radius && (distance < bestDistance || (distance == bestDistance && candidate.Id < bestId)))
                    {
                        bestId = candidate.Id;
                        bestDistance = distance;
                    }
                }
            }
        }

        return bestId;
    }

    private int BucketFor(int x, int y) => (y / BucketSize) * _bucketColumns + (x / BucketSize);
}
