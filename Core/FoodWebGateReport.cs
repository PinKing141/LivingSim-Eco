using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct FoodWebSpeciesReport(
    AnimalSpecies Species, int Initial, int Final, int Births, int MaxGeneration,
    int Kills, int CarcassMeals, long PlantConsumed, IReadOnlyDictionary<AnimalSpecies, int> KillsByPreySpecies);

public readonly record struct FoodWebGateReport(
    int Seed, int Ticks, bool Expanded, IReadOnlyList<FoodWebSpeciesReport> Species,
    long FinalBiomass, double ElapsedSeconds, string StateHash);

public static class FoodWebGateRunner
{
    public static FoodWebGateReport Run(int seed, bool expanded, int ticks = 12_000)
    {
        const int width = 128;
        const int height = 96;
        const int herbivores = 640;
        const int predators = 4;
        var simulation = HeadlessWorldRunner.CreatePopulated(
            new WorldSettings(seed, width, height), herbivores, predators, ClimateMode.MildConstant);
        for (var index = 0; index < predators; index++)
        {
            ref var founder = ref simulation.Entities.GetById(herbivores + index + 1);
            founder.Traits = new AnimalTraits { Speed = 2, Metabolism = 1, Vision = 6, Size = 3, Fertility = 1 };
            founder.Sex = index % 2 == 0 ? AnimalSex.Female : AnimalSex.Male;
        }
        var initial = new Dictionary<AnimalSpecies, int>
        {
            [AnimalSpecies.Herbivore] = herbivores,
            [AnimalSpecies.Predator] = predators,
        };
        if (expanded)
        {
            Add(AnimalSpecies.LargeHerbivore, 80);
            Add(AnimalSpecies.SmallHerbivore, 160);
            Add(AnimalSpecies.ApexPredator, 2);
            Add(AnimalSpecies.Omnivore, 8);
            Add(AnimalSpecies.Scavenger, 6);
        }
        void Add(AnimalSpecies species, int count)
        {
            HeadlessWorldRunner.AddPopulation(simulation, species, count);
            initial[species] = count;
        }

        var hunting = new HuntingTelemetry();
        var resources = new ResourceTelemetry();
        simulation.HuntingTelemetry = hunting;
        simulation.ResourceTelemetry = resources;
        var watch = Stopwatch.StartNew();
        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
        }
        watch.Stop();

        var results = new List<FoodWebSpeciesReport>();
        foreach (var species in Enum.GetValues<AnimalSpecies>())
        {
            initial.TryGetValue(species, out var started);
            var final = 0;
            var births = 0;
            var maxGeneration = 0;
            var kills = 0;
            var meals = 0;
            var killedPrey = new Dictionary<AnimalSpecies, int>();
            foreach (var animal in simulation.Entities.Items)
            {
                if (animal.Species != species) continue;
                if (animal.IsAlive) final++;
                if (animal.ParentAId != 0)
                {
                    births++;
                    maxGeneration = Math.Max(maxGeneration, animal.Generation);
                }
                if (hunting.Predators.TryGetValue(animal.Id, out var stats))
                {
                    kills += stats.Kills;
                    meals += stats.Meals;
                    foreach (var (preySpecies, count) in stats.KillsByPreySpecies)
                    {
                        killedPrey.TryGetValue(preySpecies, out var previous);
                        killedPrey[preySpecies] = previous + count;
                    }
                }
            }
            resources.PlantConsumptionBySpecies.TryGetValue(species, out var plants);
            results.Add(new FoodWebSpeciesReport(species, started, final, births, maxGeneration, kills, meals, plants, killedPrey));
        }
        long biomass = 0;
        foreach (var cell in simulation.World.Cells) biomass += cell.PlantBiomass;
        return new FoodWebGateReport(seed, ticks, expanded, results,
            biomass, watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }
}
