using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct PredatorSurvivalReport(
    int Seed,
    int Ticks,
    AnimalTraits InitialPredatorTraits,
    int EligiblePreyAtStart,
    int NearbyEligiblePreyAtStart,
    bool PredatorSurvived,
    int PredatorFinalEnergy,
    int PredatorBirths,
    int PreyAtEnd,
    int PreyDetections,
    int PursuitsStarted,
    int AbandonedPursuits,
    int Attacks,
    int Kills,
    int Meals,
    long MetabolicEnergySpent,
    long PursuitEnergySpent,
    long DirectMovementEnergySpent,
    long DirectCombatEnergySpent,
    long CarcassEnergyGained,
    long CarcassNutritionCreated,
    long KillCarcassNutritionCreated,
    long CarcassNutritionConsumed,
    long CarcassNutritionDecayed,
    long CarcassNutritionRemaining,
    long DistanceTravelled,
    int StarvationDeaths,
    double MeanTicksBetweenMeals,
    double MeanCompletedPursuitTicks,
    double ElapsedSeconds,
    string StateHash);

public static class PredatorSurvivalRunner
{
    public static PredatorSurvivalReport Run(int seed, int ticks = 6_000, int prey = 160, int width = 64, int height = 48)
    {
        // A single predator has no compatible mate, so predator reproduction is disabled
        // by the controlled setup while prey can keep reproducing.
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, width, height), prey, 1, ClimateMode.MildConstant);
        var predatorId = prey + 1;
        // Representative, viable in-range traits hold genotype constant so this gate
        // tests the hunting loop rather than a random founder's niche mismatch.
        ref var configuredPredator = ref simulation.Entities.GetById(predatorId);
        configuredPredator.Traits = new AnimalTraits { Speed = 2, Metabolism = 1, Vision = 6, Size = 3, Fertility = 1 };
        var initialPredator = simulation.Entities.GetById(predatorId);
        var eligiblePrey = 0;
        var nearbyPrey = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Species != AnimalSpecies.Herbivore || animal.Traits.Size > initialPredator.Traits.Size) continue;
            eligiblePrey++;
            if (Math.Abs(animal.X - initialPredator.X) + Math.Abs(animal.Y - initialPredator.Y) <= initialPredator.Traits.Vision * 2)
                nearbyPrey++;
        }
        var telemetry = new HuntingTelemetry();
        simulation.HuntingTelemetry = telemetry;
        var watch = Stopwatch.StartNew();
        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
        }
        watch.Stop();

        var predator = simulation.Entities.GetById(predatorId);
        var stats = telemetry.Predators.TryGetValue(predatorId, out var recorded) ? recorded : new PredatorHuntStats();
        var predatorBirths = 0;
        foreach (var animal in simulation.Entities.Items)
            if (animal.Species == AnimalSpecies.Predator && animal.ParentAId != 0) predatorBirths++;

        return new PredatorSurvivalReport(seed, ticks, initialPredator.Traits, eligiblePrey, nearbyPrey,
            predator.IsAlive, predator.Energy, predatorBirths,
            simulation.Metrics.Herbivores, stats.PreyDetections, stats.PursuitsStarted, stats.AbandonedPursuits,
            stats.Attacks, stats.Kills, stats.Meals, stats.MetabolicEnergySpent, stats.PursuitEnergySpent,
            DirectMovementEnergySpent: 0, DirectCombatEnergySpent: 0, stats.CarcassEnergyGained,
            telemetry.CarcassNutritionCreated, telemetry.KillCarcassNutritionCreated,
            telemetry.CarcassNutritionConsumed, telemetry.CarcassNutritionDecayed,
            simulation.Carcasses.Items.ToArray().Sum(carcass => (long)carcass.Nutrition),
            stats.DistanceTravelled, stats.StarvationDeaths,
            stats.Meals > 1 ? (double)stats.TotalTicksBetweenMeals / (stats.Meals - 1) : 0,
            stats.CompletedPursuits > 0 ? (double)stats.TotalPursuitTicks / stats.CompletedPursuits : 0,
            watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }
}
