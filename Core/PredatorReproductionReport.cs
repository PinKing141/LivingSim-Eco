using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct PredatorReproductionReport(
    int Seed, int Ticks, int InitialPredators, int FinalPredators, int PeakPredators,
    int Births, int OffspringReachedMaturity, int MaximumGeneration,
    int JuvenileDeaths, int JuvenileStarvationDeaths, int AdultDeaths,
    int StarvationDeaths, int EligibleFemaleSamples, int CompatibleMateSamples,
    int Kills, int FinalPrey, int LowestPrey, long? PredatorExtinctionTick,
    int LivingFemales, int LivingMales, long LastBirthTick,
    IReadOnlyList<PredatorEcologySample> Samples,
    IReadOnlyList<PredatorSizeOutcome> SizeOutcomes, double ElapsedSeconds, string StateHash);

public readonly record struct PredatorSizeOutcome(
    int Size, int Born, int Matured, int Alive, int Starved, int Kills);

public readonly record struct PredatorEcologySample(
    long Tick, int Predators, int Prey, int FemalePredators, int MalePredators,
    int Adults, int Births, int Kills, double MeanEnergy, double MeanNearbyEligiblePrey,
    long Biomass, int DepletedCells, long PlantConsumed, long PlantRegenerated, int HerbivoreStarvationDeaths,
    long NorthwestBiomass, long NortheastBiomass, long SouthwestBiomass, long SoutheastBiomass);

public static class PredatorReproductionRunner
{
    public static PredatorReproductionReport Run(int seed, int ticks = 24_000, int prey = 160, int width = 64, int height = 48, int predators = 2)
    {
        if (predators < 2) throw new ArgumentOutOfRangeException(nameof(predators));
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, width, height), prey, 0, ClimateMode.MildConstant);
        var traits = new AnimalTraits { Speed = 2, Metabolism = 1, Vision = 6, Size = 3, Fertility = 1 };
        for (var index = 0; index < predators; index++)
        {
            // Place compatible pairs near different prey patches rather than
            // stacking every founder on the same carcass at tick zero.
            var pair = index / 2;
            var anchorId = 1 + pair * Math.Max(1, prey / Math.Max(1, (predators + 1) / 2));
            var anchor = simulation.Entities.GetById(Math.Min(anchorId, prey));
            var founderId = simulation.SpawnAnimal(AnimalSpecies.Predator, Math.Min(width - 1, anchor.X + 1), anchor.Y);
            ref var founder = ref simulation.Entities.GetById(founderId);
            founder.Sex = index % 2 == 0 ? AnimalSex.Female : AnimalSex.Male;
            founder.Traits = traits;
            founder.AgeTicks = 200 + index * 500;
        }

        var telemetry = new HuntingTelemetry();
        simulation.HuntingTelemetry = telemetry;
        var resources = new ResourceTelemetry();
        simulation.ResourceTelemetry = resources;
        var peak = predators;
        var eligibleFemaleSamples = 0;
        var compatibleMateSamples = 0;
        var samples = new List<PredatorEcologySample>();
        var lowestPrey = prey;
        long? extinctionTick = null;
        var watch = Stopwatch.StartNew();
        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
            peak = Math.Max(peak, simulation.Metrics.Predators);
            lowestPrey = Math.Min(lowestPrey, simulation.Metrics.Herbivores);
            if (extinctionTick is null && simulation.Metrics.Predators == 0) extinctionTick = simulation.Tick;
            var eligibleFemales = 0;
            var eligibleMales = 0;
            foreach (var animal in simulation.Entities.Items)
            {
                if (!animal.IsAlive || animal.Species != AnimalSpecies.Predator || animal.AgeTicks < 200 || animal.Energy < 80 || animal.ReproductionCooldown != 0) continue;
                if (animal.Sex == AnimalSex.Female) eligibleFemales++;
                else eligibleMales++;
            }
            eligibleFemaleSamples += eligibleFemales;
            if (eligibleFemales > 0 && eligibleMales > 0) compatibleMateSamples++;
            if (simulation.Tick % 480 == 0)
            {
                var femaleCount = 0;
                var maleCount = 0;
                var adultCount = 0;
                var birthCount = 0;
                long energySum = 0;
                long nearbyPreySum = 0;
                foreach (var predator in simulation.Entities.Items)
                {
                    if (predator.Species != AnimalSpecies.Predator) continue;
                    if (predator.ParentAId != 0) birthCount++;
                    if (!predator.IsAlive) continue;
                    if (predator.Sex == AnimalSex.Female) femaleCount++;
                    else maleCount++;
                    if (predator.AgeTicks >= 200) adultCount++;
                    energySum += predator.Energy;
                    foreach (var candidate in simulation.Entities.Items)
                        if (candidate.IsAlive && SpeciesProfiles.IsPlantEater(candidate.Species) &&
                            candidate.Traits.Size <= predator.Traits.Size &&
                            Math.Abs(candidate.X - predator.X) + Math.Abs(candidate.Y - predator.Y) <= predator.Traits.Vision * 2)
                            nearbyPreySum++;
                }
                var living = simulation.Metrics.Predators;
                var biomass = 0L;
                var depletedCells = 0;
                var northwest = 0L;
                var northeast = 0L;
                var southwest = 0L;
                var southeast = 0L;
                for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                    {
                        var cell = simulation.World.CellAt(x, y);
                        biomass += cell.PlantBiomass;
                        if (cell.MaxPlantBiomass > 0 && cell.PlantBiomass <= cell.MaxPlantBiomass / 4) depletedCells++;
                        if (y < height / 2)
                        {
                            if (x < width / 2) northwest += cell.PlantBiomass;
                            else northeast += cell.PlantBiomass;
                        }
                        else if (x < width / 2) southwest += cell.PlantBiomass;
                        else southeast += cell.PlantBiomass;
                    }
                samples.Add(new PredatorEcologySample(simulation.Tick, living, simulation.Metrics.Herbivores,
                    femaleCount, maleCount, adultCount, birthCount,
                    telemetry.Predators.Values.Sum(stats => stats.Kills),
                    living == 0 ? 0 : (double)energySum / living,
                    living == 0 ? 0 : (double)nearbyPreySum / living,
                    biomass, depletedCells, resources.PlantConsumed, resources.PlantRegenerated,
                    resources.HerbivoreStarvationDeaths, northwest, northeast, southwest, southeast));
            }
        }
        watch.Stop();

        var births = 0;
        var mature = 0;
        var maxGeneration = 0;
        var juvenileDeaths = 0;
        var juvenileStarvation = 0;
        var adultDeaths = 0;
        var starvationDeaths = 0;
        var livingFemales = 0;
        var livingMales = 0;
        long lastBirthTick = -1;
        var sizeOutcomes = new Dictionary<int, (int Born, int Matured, int Alive, int Starved, int Kills)>();
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Species != AnimalSpecies.Predator) continue;
            sizeOutcomes.TryGetValue(animal.Traits.Size, out var sizeStats);
            if (animal.ParentAId != 0) sizeStats.Born++;
            if (animal.ParentAId != 0 && animal.AgeTicks >= 200) sizeStats.Matured++;
            if (animal.IsAlive) sizeStats.Alive++;
            if (!animal.IsAlive && animal.Energy <= 0) sizeStats.Starved++;
            if (telemetry.Predators.TryGetValue(animal.Id, out var huntStats)) sizeStats.Kills += huntStats.Kills;
            sizeOutcomes[animal.Traits.Size] = sizeStats;
            if (animal.IsAlive)
            {
                if (animal.Sex == AnimalSex.Female) livingFemales++;
                else livingMales++;
            }
            if (animal.ParentAId != 0)
            {
                births++;
                lastBirthTick = Math.Max(lastBirthTick, simulation.Lineages.Records[animal.Id].BirthTick);
                if (animal.AgeTicks >= 200) mature++;
                maxGeneration = Math.Max(maxGeneration, animal.Generation);
            }
            if (!animal.IsAlive)
            {
                if (animal.AgeTicks < 200)
                {
                    juvenileDeaths++;
                    if (animal.Energy <= 0) juvenileStarvation++;
                }
                else adultDeaths++;
                if (animal.Energy <= 0) starvationDeaths++;
            }
        }
        var kills = telemetry.Predators.Values.Sum(stats => stats.Kills);
        return new PredatorReproductionReport(seed, ticks, predators, simulation.Metrics.Predators, peak, births,
            mature, maxGeneration, juvenileDeaths, juvenileStarvation, adultDeaths, starvationDeaths,
            eligibleFemaleSamples, compatibleMateSamples, kills, simulation.Metrics.Herbivores, lowestPrey,
            extinctionTick, livingFemales, livingMales, lastBirthTick, samples,
            sizeOutcomes.OrderBy(entry => entry.Key).Select(entry => new PredatorSizeOutcome(
                entry.Key, entry.Value.Born, entry.Value.Matured, entry.Value.Alive,
                entry.Value.Starved, entry.Value.Kills)).ToArray(),
            watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }
}
