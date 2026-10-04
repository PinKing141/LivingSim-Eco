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
            Energy = species == AnimalSpecies.Herbivore ? 80 : 120,
            Health = species == AnimalSpecies.Herbivore ? 40 : 60,
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

    private void EnsureCapacity()
    {
        if (_count < _items.Length)
        {
            return;
        }

        Array.Resize(ref _items, _items.Length * 2);
    }
}
