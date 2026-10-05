using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct PredatorReproductionReport(
    int Seed, int Ticks, int InitialPredators, int FinalPredators, int PeakPredators,
    int Births, int OffspringReachedMaturity, int MaximumGeneration,
    int JuvenileDeaths, int JuvenileStarvationDeaths, int AdultDeaths,
    int StarvationDeaths, int EligibleFemaleSamples, int CompatibleMateSamples,
    int Kills, int FinalPrey, int LowestPrey, long? PredatorExtinctionTick,
    int LivingFemales, int LivingMales, long LastBirthTick, double ElapsedSeconds, string StateHash);

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
        }

        var telemetry = new HuntingTelemetry();
        simulation.HuntingTelemetry = telemetry;
        var peak = predators;
        var eligibleFemaleSamples = 0;
        var compatibleMateSamples = 0;
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
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Species != AnimalSpecies.Predator) continue;
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
            extinctionTick, livingFemales, livingMales, lastBirthTick,
            watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }
}
