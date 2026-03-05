using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LivingSim.Core;
using LivingSim.World;
using LivingSim.Environment;
using LivingSim.Generation;
using LivingSim.Observation;
using LivingSim.Visualisation;
using LivingSim.Animals;
using LivingSim.Config;

class Program
{
    static void Main()
    {
        // ----------------------------
        // 1. Create core objects
        // ----------------------------
        var config = new SimulationConfig();
        var random = new Random(config.RandomSeed);
        var clock = new SimulationClock();
        var grid = new Grid(width: config.WorldWidth, height: config.WorldHeight);
        var environment = new EnvironmentTickSystem();
        var metrics = new MetricsCollector();
        var idGenerator = new DeterministicIdGenerator(config.RandomSeed);
        var animals = new AnimalManager(grid.Width, grid.Height, random, idGenerator);
        var visualizer = new ConsoleVisualizer();
        var metricsRenderer = new ConsoleMetricsRenderer();

        // ----------------------------
        // 2. Generate world
        // ----------------------------
        // The WorldGenerator now uses the shared 'random' instance to ensure
        // the entire simulation is deterministic from a single source.
        var generator = new WorldGenerator(random);
        generator.Generate(grid, (g) =>
        {
            visualizer.Draw(g, new List<Animal>(), clock, new List<Dictionary<Species, int>>(), false);
        }, config.WorldGenerationSteps);

        // ----------------------------
        // 3. Spawn some animals
        // ----------------------------
        // Spawn a small pack of wolves
        for (int i = 0; i < config.InitialCarnivores; i++)
        {
            animals.SpawnAnimal(AnimalType.Carnivore, random.Next(grid.Width), random.Next(grid.Height));
        }
        // Spawn a herd of herbivores
        for (int i = 0; i < config.InitialHerbivores; i++)
        {
            animals.SpawnAnimal(AnimalType.Herbivore, random.Next(grid.Width), random.Next(grid.Height));
        }
        // Spawn a group of omnivores
        for (int i = 0; i < config.InitialOmnivores; i++)
        {
            animals.SpawnAnimal(AnimalType.Omnivore, random.Next(grid.Width), random.Next(grid.Height));
        }

        // ----------------------------
        // 4. Create world manager
        // ----------------------------
        var manager = new WorldManager(clock, grid, environment, metrics, animals);

        // ----------------------------
        // 5. Run simulation
        // ----------------------------
        int simulationDelay = config.InitialSimulationDelayMs;
        bool isPaused = false;
        bool showStats = false;
        List<Dictionary<Species, int>> history = new List<Dictionary<Species, int>>();
        Console.CursorVisible = false;
        for (int i = 0; i < config.SimulationTicks; )
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow) simulationDelay = Math.Max(0, simulationDelay - config.DelayAdjustmentStepMs);
                if (key == ConsoleKey.DownArrow) simulationDelay += config.DelayAdjustmentStepMs;
                if (key == ConsoleKey.Spacebar) isPaused = !isPaused;
                if (key == ConsoleKey.S) showStats = !showStats;
            }

            visualizer.Draw(grid, animals.GetAnimals(), clock, history, showStats);
            Console.WriteLine($"Delay: {simulationDelay}ms (Up: Faster, Down: Slower, Space: Pause, S: Stats) {(isPaused ? "[PAUSED]" : "")}".PadRight(Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 80));
            
            if (!isPaused)
            {
                manager.Tick();

                // Record population history
                var currentStats = animals.GetAnimals()
                    .Where(a => a.IsAlive)
                    .GroupBy(a => a.Species)
                    .ToDictionary(g => g.Key, g => g.Count());
                history.Add(currentStats);
                if (history.Count > config.PopulationHistoryLength) history.RemoveAt(0);
                if (config.RenderMetricsToConsole)
                {
                    metricsRenderer.Render(metrics.CurrentSnapshot);
                }

                Thread.Sleep(simulationDelay);
                i++;
            }
            else
            {
                Thread.Sleep(config.PausedDelayMs);
            }
        }
        Console.CursorVisible = true;

        Console.WriteLine("Simulation complete.");
    }
}
