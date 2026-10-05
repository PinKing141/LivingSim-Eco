using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct PreyViabilityReport(
    int Seed,
    int Ticks,
    int InitialPopulation,
    int FinalPopulation,
    int LowestPopulation,
    int HighestPopulation,
    int Births,
    int OffspringReachedMaturity,
    int MaximumGeneration,
    int StarvationDeaths,
    int AgeDeaths,
    int DepletedCells,
    int RecoveredCells,
    int MaximumHabitatPressure,
    long InitialBiomass,
    long FinalBiomass,
    double ElapsedSeconds,
    string StateHash);

public static class PreyViabilityRunner
{
    public static PreyViabilityReport Run(int seed, int ticks = 48_000, int herbivores = 24, int width = 64, int height = 48)
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, width, height), herbivores, 0, ClimateMode.MildConstant);
        var depleted = new bool[simulation.World.CellCount];
        var recovered = new bool[simulation.World.CellCount];
        var initialBiomass = Biomass(simulation);
        var low = herbivores;
        var high = herbivores;
        var maximumPressure = 0;
        var watch = Stopwatch.StartNew();

        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
            low = Math.Min(low, simulation.Metrics.Herbivores);
            high = Math.Max(high, simulation.Metrics.Herbivores);
            var index = 0;
            foreach (var cell in simulation.World.Cells)
            {
                maximumPressure = Math.Max(maximumPressure, cell.HabitatPressure);
                if (cell.MaxPlantBiomass > 0)
                {
                    if (cell.PlantBiomass <= cell.MaxPlantBiomass / 4) depleted[index] = true;
                    else if (depleted[index] && cell.PlantBiomass >= cell.MaxPlantBiomass / 2) recovered[index] = true;
                }
                index++;
            }
        }

        watch.Stop();
        var births = 0;
        var matureOffspring = 0;
        var maximumGeneration = 0;
        var starvationDeaths = 0;
        var ageDeaths = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.ParentAId != 0)
            {
                births++;
                if (animal.AgeTicks >= 200) matureOffspring++;
                maximumGeneration = Math.Max(maximumGeneration, animal.Generation);
            }
            if (!animal.IsAlive)
            {
                if (animal.Energy <= 0) starvationDeaths++;
                else ageDeaths++;
            }
        }

        return new PreyViabilityReport(seed, ticks, herbivores, simulation.Metrics.Herbivores, low, high,
            births, matureOffspring, maximumGeneration, starvationDeaths, ageDeaths,
            depleted.Count(value => value), recovered.Count(value => value), maximumPressure,
            initialBiomass, Biomass(simulation), watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }

    private static long Biomass(WorldSimulation simulation)
    {
        long biomass = 0;
        foreach (var cell in simulation.World.Cells) biomass += cell.PlantBiomass;
        return biomass;
    }
}
