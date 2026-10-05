using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct TraitDistribution(
    int Population,
    IReadOnlyDictionary<int, int> Speed,
    IReadOnlyDictionary<int, int> Metabolism,
    IReadOnlyDictionary<int, int> Vision,
    IReadOnlyDictionary<int, int> Size,
    IReadOnlyDictionary<int, int> Fertility)
{
    public static TraitDistribution Capture(WorldSimulation simulation, AnimalSpecies species)
    {
        var speed = new Dictionary<int, int>();
        var metabolism = new Dictionary<int, int>();
        var vision = new Dictionary<int, int>();
        var size = new Dictionary<int, int>();
        var fertility = new Dictionary<int, int>();
        var count = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (!animal.IsAlive || animal.Species != species) continue;
            count++;
            Add(speed, animal.Traits.Speed);
            Add(metabolism, animal.Traits.Metabolism);
            Add(vision, animal.Traits.Vision);
            Add(size, animal.Traits.Size);
            Add(fertility, animal.Traits.Fertility);
        }
        return new TraitDistribution(count, speed, metabolism, vision, size, fertility);
    }

    private static void Add(Dictionary<int, int> values, int trait)
    {
        values.TryGetValue(trait, out var count);
        values[trait] = count + 1;
    }
}

public readonly record struct EvolutionGateReport(
    int Seed, ClimateMode ClimateMode, int Ticks, TraitDistribution Initial,
    TraitDistribution Final, int Births, int MatureOffspring, int MaximumGeneration,
    int InheritanceViolations, int TraitBoundViolations, double ElapsedSeconds, string StateHash);

public static class EvolutionGateRunner
{
    public static EvolutionGateReport Run(int seed, ClimateMode climateMode, int ticks = 48_000)
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, 96, 64), 96, 0, climateMode);
        var initial = TraitDistribution.Capture(simulation, AnimalSpecies.Herbivore);
        var watch = Stopwatch.StartNew();
        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
        }
        watch.Stop();
        var births = 0;
        var mature = 0;
        var generation = 0;
        var inheritanceViolations = 0;
        var boundViolations = 0;
        var bounds = SpeciesProfiles.For(AnimalSpecies.Herbivore);
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Species != AnimalSpecies.Herbivore) continue;
            if (!WithinBounds(animal.Traits, bounds)) boundViolations++;
            if (animal.ParentAId == 0) continue;
            births++;
            if (animal.AgeTicks >= 200) mature++;
            generation = Math.Max(generation, animal.Generation);
            var mother = simulation.Entities.GetById(animal.ParentAId);
            var father = simulation.Entities.GetById(animal.ParentBId);
            if (!InheritedWithinMutation(animal.Traits, mother.Traits, father.Traits)) inheritanceViolations++;
        }
        return new EvolutionGateReport(seed, climateMode, ticks, initial,
            TraitDistribution.Capture(simulation, AnimalSpecies.Herbivore), births, mature, generation,
            inheritanceViolations, boundViolations, watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }

    private static bool WithinBounds(AnimalTraits traits, SpeciesProfile bounds) =>
        Between(traits.Speed, bounds.MinimumTraits.Speed, bounds.MaximumTraits.Speed) &&
        Between(traits.Metabolism, bounds.MinimumTraits.Metabolism, bounds.MaximumTraits.Metabolism) &&
        Between(traits.Vision, bounds.MinimumTraits.Vision, bounds.MaximumTraits.Vision) &&
        Between(traits.Size, bounds.MinimumTraits.Size, bounds.MaximumTraits.Size) &&
        Between(traits.Fertility, bounds.MinimumTraits.Fertility, bounds.MaximumTraits.Fertility);

    private static bool Between(int value, int minimum, int maximum) => value >= minimum && value <= maximum;

    private static bool InheritedWithinMutation(AnimalTraits child, AnimalTraits mother, AnimalTraits father) =>
        WithinParentalRange(child.Speed, mother.Speed, father.Speed) &&
        WithinParentalRange(child.Metabolism, mother.Metabolism, father.Metabolism) &&
        WithinParentalRange(child.Vision, mother.Vision, father.Vision) &&
        WithinParentalRange(child.Size, mother.Size, father.Size) &&
        WithinParentalRange(child.Fertility, mother.Fertility, father.Fertility);

    private static bool WithinParentalRange(int child, int mother, int father)
    {
        var lowerMean = (mother + father) / 2;
        var upperMean = (mother + father + 1) / 2;
        return child >= lowerMean - 1 && child <= upperMean + 1;
    }
}
