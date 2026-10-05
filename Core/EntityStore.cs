namespace LivingSim.Core;

public sealed class EntityStore
{
    private AnimalState[] _items = new AnimalState[64];
    private int _count;

    public int Count => _count;
    public ReadOnlySpan<AnimalState> Items => _items.AsSpan(0, _count);

    public int Spawn(AnimalSpecies species, int x, int y)
    {
        EnsureCapacity();
        var id = _count + 1;
        _items[_count++] = new AnimalState
        {
            Id = id,
            Species = species,
            X = x,
            Y = y,
            Energy = 100,
            Health = 50,
            IsAlive = true,
            TargetEntityId = 0,
            TargetX = x,
            TargetY = y,
        };
        return id;
    }

    public ref AnimalState GetById(int id)
    {
        if (id is < 1 or > int.MaxValue || id > _count)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        return ref _items[id - 1];
    }

    public void SetEnergy(int id, int energy) => GetById(id).Energy = energy;

    internal void Restore(IEnumerable<AnimalState> animals)
    {
        _count = 0;
        foreach (var animal in animals.OrderBy(animal => animal.Id))
        {
            if (animal.Id != _count + 1)
            {
                throw new InvalidDataException("Saved animal IDs must be contiguous and start at one.");
            }

            EnsureCapacity();
            _items[_count++] = animal;
        }
    }

    private void EnsureCapacity()
    {
        if (_count < _items.Length)
        {
            return;
        }

        Array.Resize(ref _items, _items.Length * 2);
    }
}
