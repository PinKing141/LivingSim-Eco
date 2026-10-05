using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct ClimateGateReport(
    int Seed,
    ClimateMode Mode,
    int Ticks,
    int InitialHerbivores,
    int FinalHerbivores,
    int HerbivoreLow,
    int HerbivoreBirths,
    int MaximumGeneration,
    long InitialBiomass,
    long FinalBiomass,
    long MinimumBiomass,
    long MaximumBiomass,
    int MinimumClimateIndex,
    int MaximumClimateIndex,
    int ClimateTransitions,
    double ElapsedSeconds,
    string StateHash);

public static class ClimateGateRunner
{
    public static ClimateGateReport Run(int seed, ClimateMode mode, int ticks = 48_000)
    {
        const int herbivores = 96;
        const int predators = 0;
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, 96, 64), herbivores, predators, mode);
        var initialBiomass = TotalBiomass(simulation);
        var minimumBiomass = initialBiomass;
        var maximumBiomass = initialBiomass;
        var herbivoreLow = herbivores;
        var minimumClimate = simulation.Climate.ClimateIndexMilli;
        var maximumClimate = minimumClimate;
        var transitions = 0;
        var priorClimateSign = Math.Sign(minimumClimate);
        var watch = Stopwatch.StartNew();

        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
            var biomass = TotalBiomass(simulation);
            minimumBiomass = Math.Min(minimumBiomass, biomass);
            maximumBiomass = Math.Max(maximumBiomass, biomass);
            herbivoreLow = Math.Min(herbivoreLow, simulation.Metrics.Herbivores);
            minimumClimate = Math.Min(minimumClimate, simulation.Climate.ClimateIndexMilli);
            maximumClimate = Math.Max(maximumClimate, simulation.Climate.ClimateIndexMilli);
            var climateSign = Math.Sign(simulation.Climate.ClimateIndexMilli);
            if (climateSign != 0 && priorClimateSign != 0 && climateSign != priorClimateSign)
                transitions++;
            if (climateSign != 0)
                priorClimateSign = climateSign;
        }

        watch.Stop();
        var births = 0;
        var generation = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Species != AnimalSpecies.Herbivore || animal.ParentAId == 0) continue;
            births++;
            generation = Math.Max(generation, animal.Generation);
        }

        return new ClimateGateReport(seed, mode, ticks, herbivores, simulation.Metrics.Herbivores, herbivoreLow,
            births, generation, initialBiomass, TotalBiomass(simulation), minimumBiomass, maximumBiomass,
            minimumClimate, maximumClimate, transitions, watch.Elapsed.TotalSeconds,
            SimulationStateHasher.Hash(simulation));
    }

    private static long TotalBiomass(WorldSimulation simulation)
    {
        long total = 0;
        foreach (var cell in simulation.World.Cells)
            total += cell.PlantBiomass;
        return total;
    }
}
