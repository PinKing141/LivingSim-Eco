using LivingSim.Core;
using LivingSim.Observation;

var options = HeadlessOptions.Parse(args);
if (options.PredatorReproductionGate)
{
    var reproduction = PredatorReproductionRunner.Run(options.Settings.Seed, options.Ticks, options.Herbivores, options.Settings.Width, options.Settings.Height, options.Predators);
    Console.WriteLine($"gate=4 seed={reproduction.Seed} ticks={reproduction.Ticks} predators={reproduction.InitialPredators}->{reproduction.FinalPredators} living-sex={reproduction.LivingFemales}F/{reproduction.LivingMales}M peak={reproduction.PeakPredators} extinct={reproduction.PredatorExtinctionTick?.ToString() ?? "never"} prey={reproduction.FinalPrey} prey-low={reproduction.LowestPrey} births={reproduction.Births} last-birth={reproduction.LastBirthTick} mature-offspring={reproduction.OffspringReachedMaturity} max-generation={reproduction.MaximumGeneration} juvenile-deaths={reproduction.JuvenileDeaths} juvenile-starvation={reproduction.JuvenileStarvationDeaths} adult-deaths={reproduction.AdultDeaths} starvation={reproduction.StarvationDeaths} eligible-female-samples={reproduction.EligibleFemaleSamples} compatible-samples={reproduction.CompatibleMateSamples} kills={reproduction.Kills} seconds={reproduction.ElapsedSeconds:F2} state={reproduction.StateHash}");
    return;
}
if (options.PredatorGate)
{
    var hunt = PredatorSurvivalRunner.Run(options.Settings.Seed, options.Ticks, options.Herbivores, options.Settings.Width, options.Settings.Height);
    Console.WriteLine($"gate=2 seed={hunt.Seed} ticks={hunt.Ticks} traits=size{hunt.InitialPredatorTraits.Size}/vision{hunt.InitialPredatorTraits.Vision}/speed{hunt.InitialPredatorTraits.Speed}/metabolism{hunt.InitialPredatorTraits.Metabolism} eligible-prey={hunt.EligiblePreyAtStart} nearby-eligible={hunt.NearbyEligiblePreyAtStart} alive={hunt.PredatorSurvived} energy={hunt.PredatorFinalEnergy} predator-births={hunt.PredatorBirths} prey={hunt.PreyAtEnd} detections={hunt.PreyDetections} pursuits={hunt.PursuitsStarted} abandoned={hunt.AbandonedPursuits} attacks={hunt.Attacks} kills={hunt.Kills} meals={hunt.Meals} upkeep={hunt.MetabolicEnergySpent} pursuit-upkeep={hunt.PursuitEnergySpent} direct-move-cost={hunt.DirectMovementEnergySpent} direct-combat-cost={hunt.DirectCombatEnergySpent} carcass-gain={hunt.CarcassEnergyGained} carcass-created={hunt.CarcassNutritionCreated} kill-carcass-created={hunt.KillCarcassNutritionCreated} carcass-consumed={hunt.CarcassNutritionConsumed} carcass-decayed={hunt.CarcassNutritionDecayed} carcass-remaining={hunt.CarcassNutritionRemaining} distance={hunt.DistanceTravelled} starvation-deaths={hunt.StarvationDeaths} ticks-between-meals={hunt.MeanTicksBetweenMeals:F1} pursuit-duration={hunt.MeanCompletedPursuitTicks:F1} seconds={hunt.ElapsedSeconds:F2} state={hunt.StateHash}");
    return;
}
if (options.PreyGate)
{
    var prey = PreyViabilityRunner.Run(options.Settings.Seed, options.Ticks, options.Herbivores, options.Settings.Width, options.Settings.Height);
    Console.WriteLine($"gate=1 seed={prey.Seed} ticks={prey.Ticks} prey={prey.InitialPopulation}->{prey.FinalPopulation} range={prey.LowestPopulation}-{prey.HighestPopulation} births={prey.Births} mature-offspring={prey.OffspringReachedMaturity} max-generation={prey.MaximumGeneration} starvation={prey.StarvationDeaths} age-deaths={prey.AgeDeaths} depleted-cells={prey.DepletedCells} recovered-cells={prey.RecoveredCells} max-pressure={prey.MaximumHabitatPressure} biomass={prey.InitialBiomass}->{prey.FinalBiomass} seconds={prey.ElapsedSeconds:F2} state={prey.StateHash}");
    return;
}
if (options.FoundationGate)
{
    var gate = FoundationGateRunner.Run(options.Settings.Seed, options.Ticks, options.Herbivores, options.Predators, options.Settings.Width, options.Settings.Height);
    Console.WriteLine($"seed={gate.Seed} ticks={gate.Ticks} herbivores={gate.InitialHerbivores}->{gate.FinalHerbivores} range={gate.HerbivoreLow}-{gate.HerbivoreHigh} predators={gate.InitialPredators}->{gate.FinalPredators} high={gate.PredatorHigh} extinct={gate.PredatorExtinctionTick?.ToString() ?? "never"} births={gate.HerbivoreBirths}/{gate.PredatorBirths} gen={gate.MaxGeneration} metabolism={gate.InitialMetabolism:F2}->{gate.FinalMetabolism:F2} reversals={gate.PopulationReversals} climate-eras={gate.ClimateEraCount} history={gate.HistoryEvents} allocated-mb={gate.AllocatedBytes / 1_048_576.0:F1} seconds={gate.ElapsedSeconds:F2} state={gate.StateHash}");
    return;
}
if (options.Benchmark is not null)
{
    var result = ReleaseBenchmarks.Run(ReleaseBenchmarks.Find(options.Benchmark), options.Ticks);
    Console.WriteLine($"benchmark={result.Name} ticks={result.Ticks} created={result.AnimalsCreated} living={result.LivingAnimals} elapsed-ms={result.Elapsed.TotalMilliseconds:F0} allocated-mb={result.AllocatedBytes / 1_048_576.0:F1} history={result.NaturalHistoryEvents} state={result.StateHash}");
    return;
}

var simulation = options.LoadPath is null
    ? HeadlessWorldRunner.CreatePopulated(options.Settings, options.Herbivores, options.Predators)
    : SimulationSaveService.Load(options.LoadPath);
if (options.LoadPath is null)
{
    HeadlessWorldRunner.AddPopulation(simulation, AnimalSpecies.LargeHerbivore, options.LargeHerbivores);
    HeadlessWorldRunner.AddPopulation(simulation, AnimalSpecies.SmallHerbivore, options.SmallHerbivores);
    HeadlessWorldRunner.AddPopulation(simulation, AnimalSpecies.ApexPredator, options.ApexPredators);
    HeadlessWorldRunner.AddPopulation(simulation, AnimalSpecies.Omnivore, options.Omnivores);
    HeadlessWorldRunner.AddPopulation(simulation, AnimalSpecies.Scavenger, options.Scavengers);
}
if (options.Observe)
{
    ObserverConsole.Run(simulation);
    if (options.SavePath is not null) SimulationSaveService.Save(simulation, options.SavePath);
    return;
}

var report = HeadlessWorldRunner.RunWithReport(simulation, options.Ticks);

Console.WriteLine($"version={SimulationVersion.Value}");
Console.WriteLine($"seed={simulation.World.Settings.Seed} size={simulation.World.Width}x{simulation.World.Height} ticks={simulation.Tick}");
Console.WriteLine($"population herbivores={simulation.Metrics.Herbivores} predators={simulation.Metrics.Predators} carcasses={simulation.Metrics.Carcasses}");
Console.WriteLine($"species {string.Join(", ", simulation.PopulationBySpecies.OrderBy(entry => entry.Key).Select(entry => $"{entry.Key}={entry.Value}"))}");
Console.WriteLine($"herbivore traits speed={simulation.Metrics.HerbivoreTraits.Speed:F2} metabolism={simulation.Metrics.HerbivoreTraits.Metabolism:F2} vision={simulation.Metrics.HerbivoreTraits.Vision:F2} size={simulation.Metrics.HerbivoreTraits.Size:F2} fertility={simulation.Metrics.HerbivoreTraits.Fertility:F2} generation={simulation.Metrics.HerbivoreTraits.Generation:F2}");
Console.WriteLine($"ecology samples={report.Samples.Count} signals={report.Signals}");
Console.WriteLine($"groups={simulation.Groups.Count} migrating={simulation.Groups.Values.Count(group => group.IsMigrating)}");
Console.WriteLine($"climate season={simulation.Climate.Season} index={simulation.Climate.ClimateIndexMilli} biomass-modifier={simulation.Climate.BiomassModifierMilli} history={simulation.ClimateHistory.Records.Count}");
Console.WriteLine($"natural-history events={simulation.NaturalHistory.Events.Count} lineage-records={simulation.Lineages.Records.Count}");
Console.WriteLine($"state={SimulationStateHasher.Hash(simulation)}");
if (options.SavePath is not null)
{
    SimulationSaveService.Save(simulation, options.SavePath);
    Console.WriteLine($"saved={Path.GetFullPath(options.SavePath)}");
}

internal sealed record HeadlessOptions(WorldSettings Settings, int Ticks, int Herbivores, int Predators, int LargeHerbivores, int SmallHerbivores, int ApexPredators, int Omnivores, int Scavengers, bool Observe, string? SavePath, string? LoadPath, string? Benchmark, bool FoundationGate, bool PreyGate, bool PredatorGate, bool PredatorReproductionGate)
{
    public static HeadlessOptions Parse(string[] args)
    {
        var seed = 1;
        var width = 96;
        var height = 64;
        var ticks = 1_000;
        var herbivores = 24;
        var predators = 6;
        var largeHerbivores = 0;
        var smallHerbivores = 0;
        var apexPredators = 0;
        var omnivores = 0;
        var scavengers = 0;
        var observe = false;
        var foundationGate = false;
        var preyGate = false;
        var predatorGate = false;
        var predatorReproductionGate = false;
        string? savePath = null;
        string? loadPath = null;
        string? benchmark = null;

        for (var index = 0; index < args.Length;)
        {
            if (args[index] == "--observe")
            {
                observe = true;
                index++;
                continue;
            }
            if (args[index] == "--foundation-gate")
            {
                foundationGate = true;
                index++;
                continue;
            }
            if (args[index] == "--prey-gate")
            {
                preyGate = true;
                index++;
                continue;
            }
            if (args[index] == "--predator-gate")
            {
                predatorGate = true;
                index++;
                continue;
            }
            if (args[index] == "--predator-reproduction-gate")
            {
                predatorReproductionGate = true;
                index++;
                continue;
            }

            if (index + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing value for {args[index]}.");
            }

            if (args[index] is "--save" or "--load" or "--benchmark")
            {
                switch (args[index])
                {
                    case "--save": savePath = args[index + 1]; break;
                    case "--load": loadPath = args[index + 1]; break;
                    case "--benchmark": benchmark = args[index + 1]; break;
                }
                index += 2;
                continue;
            }

            var value = int.Parse(args[index + 1], System.Globalization.CultureInfo.InvariantCulture);
            switch (args[index])
            {
                case "--seed": seed = value; break;
                case "--width": width = value; break;
                case "--height": height = value; break;
                case "--ticks": ticks = value; break;
                case "--herbivores": herbivores = value; break;
                case "--predators": predators = value; break;
                case "--large-herbivores": largeHerbivores = value; break;
                case "--small-herbivores": smallHerbivores = value; break;
                case "--apex-predators": apexPredators = value; break;
                case "--omnivores": omnivores = value; break;
                case "--scavengers": scavengers = value; break;
                default: throw new ArgumentException($"Unknown option {args[index]}.");
            }

            index += 2;
        }

        return new HeadlessOptions(new WorldSettings(seed, width, height), ticks, herbivores, predators, largeHerbivores, smallHerbivores, apexPredators, omnivores, scavengers, observe, savePath, loadPath, benchmark, foundationGate, preyGate, predatorGate, predatorReproductionGate);
    }
}
