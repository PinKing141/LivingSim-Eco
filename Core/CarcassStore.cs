namespace LivingSim.Core;

public sealed class CarcassStore
{
    private readonly List<Carcass> _items = [];

    public int Count => _items.Count;
    public ReadOnlySpan<Carcass> Items => System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_items);

    internal void Add(AnimalSpecies species, int x, int y) => _items.Add(new Carcass
    {
        Species = species,
        X = x,
        Y = y,
        Nutrition = species == AnimalSpecies.Herbivore ? 60 : 90,
    });

    internal ref Carcass Get(int index) => ref System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_items)[index];

    internal void RemoveAt(int index) => _items.RemoveAt(index);
}
