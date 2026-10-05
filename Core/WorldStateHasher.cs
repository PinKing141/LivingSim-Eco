namespace LivingSim.Core;

public static class WorldStateHasher
{
    public static string Hash(World world, long tick)
    {
        ulong hash = 14695981039346656037UL;
        Add(ref hash, unchecked((uint)world.Settings.Seed));
        Add(ref hash, unchecked((uint)world.Width));
        Add(ref hash, unchecked((uint)world.Height));
        Add(ref hash, unchecked((ulong)tick));

        foreach (var cell in world.Cells)
        {
            Add(ref hash, (byte)cell.Terrain);
            Add(ref hash, (byte)cell.Biome);
            Add(ref hash, unchecked((uint)cell.PlantBiomass));
            Add(ref hash, unchecked((uint)cell.MaxPlantBiomass));
            Add(ref hash, unchecked((uint)cell.BiomassRegenerationPerTick));
            Add(ref hash, unchecked((uint)cell.HabitatPressure));
        }

        return hash.ToString("x16", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void Add(ref ulong hash, byte value)
    {
        hash ^= value;
        hash *= 1099511628211UL;
    }

    private static void Add(ref ulong hash, uint value)
    {
        for (var shift = 0; shift < 32; shift += 8)
        {
            Add(ref hash, (byte)(value >> shift));
        }
    }

    private static void Add(ref ulong hash, ulong value)
    {
        for (var shift = 0; shift < 64; shift += 8)
        {
            Add(ref hash, (byte)(value >> shift));
        }
    }
}
