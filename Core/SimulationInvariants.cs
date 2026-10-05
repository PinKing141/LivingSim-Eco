namespace LivingSim.Core;

/// <summary>Diagnostic checks for gate runs; not called by the simulation hot path.</summary>
public static class SimulationInvariants
{
    public static void Validate(WorldSimulation simulation)
    {
        if (simulation.Tick < 0) throw new InvalidDataException("Negative simulation tick.");
        foreach (var cell in simulation.World.Cells)
        {
            if (cell.MaxPlantBiomass < 0 || cell.PlantBiomass < 0 || cell.PlantBiomass > cell.MaxPlantBiomass || cell.HabitatPressure is < 0 or > 1_000)
                throw new InvalidDataException("World cell biomass or pressure is outside its bounds.");
        }

        var living = 0;
        var nextId = 1;
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Id != nextId++ || (uint)animal.X >= (uint)simulation.World.Width || (uint)animal.Y >= (uint)simulation.World.Height)
                throw new InvalidDataException("Animal ID or location is invalid.");
            if (!animal.IsAlive) continue;
            if (animal.Energy <= 0 || animal.Health <= 0) throw new InvalidDataException("Living animal has nonpositive energy or health.");
            var profile = SpeciesProfiles.For(animal.Species);
            if (!InRange(animal.Traits.Speed, profile.MinimumTraits.Speed, profile.MaximumTraits.Speed) ||
                !InRange(animal.Traits.Metabolism, profile.MinimumTraits.Metabolism, profile.MaximumTraits.Metabolism) ||
                !InRange(animal.Traits.Vision, profile.MinimumTraits.Vision, profile.MaximumTraits.Vision) ||
                !InRange(animal.Traits.Size, profile.MinimumTraits.Size, profile.MaximumTraits.Size) ||
                !InRange(animal.Traits.Fertility, profile.MinimumTraits.Fertility, profile.MaximumTraits.Fertility))
                throw new InvalidDataException("Living animal trait exceeds its species bounds.");
            living++;
        }

        if (simulation.PopulationBySpecies.Values.Sum() != living)
            throw new InvalidDataException("Population metrics disagree with living animals.");
        if (!double.IsFinite(simulation.Metrics.HerbivoreTraits.Metabolism) ||
            !double.IsFinite(simulation.Metrics.PredatorTraits.Metabolism))
            throw new InvalidDataException("Trait metrics contain a non-finite value.");
        foreach (var carcass in simulation.Carcasses.Items)
            if (carcass.Nutrition <= 0 || (uint)carcass.X >= (uint)simulation.World.Width || (uint)carcass.Y >= (uint)simulation.World.Height)
                throw new InvalidDataException("Carcass state is invalid.");
    }

    private static bool InRange(int value, int minimum, int maximum) => value >= minimum && value <= maximum;
}
