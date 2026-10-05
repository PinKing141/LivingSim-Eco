namespace LivingSim.Core;

public static class SimulationStateHasher
{
    public static string Hash(WorldSimulation simulation)
    {
        ulong hash = 14695981039346656037UL;
        Add(ref hash, WorldStateHasher.Hash(simulation.World, simulation.Tick));
        Add(ref hash, (int)simulation.ClimateMode);
        Add(ref hash, (int)simulation.Climate.Season);
        Add(ref hash, simulation.Climate.ClimateIndexMilli);
        Add(ref hash, simulation.Climate.BiomassModifierMilli);
        foreach (var animal in simulation.Entities.Items)
        {
            Add(ref hash, animal.Id);
            Add(ref hash, (int)animal.Species);
            Add(ref hash, animal.X);
            Add(ref hash, animal.Y);
            Add(ref hash, animal.Energy);
            Add(ref hash, animal.Health);
            Add(ref hash, animal.AgeTicks);
            Add(ref hash, animal.Generation);
            Add(ref hash, animal.ParentAId);
            Add(ref hash, animal.ParentBId);
            Add(ref hash, animal.GroupId);
            Add(ref hash, animal.ReproductionCooldown);
            Add(ref hash, (int)animal.Sex);
            Add(ref hash, animal.Traits.Speed);
            Add(ref hash, animal.Traits.Metabolism);
            Add(ref hash, animal.Traits.Vision);
            Add(ref hash, animal.Traits.Size);
            Add(ref hash, animal.Traits.Fertility);
            Add(ref hash, animal.IsAlive ? 1 : 0);
            Add(ref hash, animal.CarcassCreated ? 1 : 0);
        }

        foreach (var carcass in simulation.Carcasses.Items)
        {
            Add(ref hash, (int)carcass.Species);
            Add(ref hash, carcass.X);
            Add(ref hash, carcass.Y);
            Add(ref hash, carcass.Nutrition);
        }

        return hash.ToString("x16", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void Add(ref ulong hash, string value)
    {
        foreach (var character in value)
        {
            Add(ref hash, character);
        }
    }

    private static void Add(ref ulong hash, int value)
    {
        unchecked
        {
            for (var shift = 0; shift < 32; shift += 8)
            {
                Add(ref hash, (byte)(value >> shift));
            }
        }
    }

    private static void Add(ref ulong hash, char value) => Add(ref hash, (byte)value);

    private static void Add(ref ulong hash, byte value)
    {
        hash ^= value;
        hash *= 1099511628211UL;
    }
}
