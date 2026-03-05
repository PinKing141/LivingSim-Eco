using LivingSim.World;
using LivingSim.Core;

namespace LivingSim.Observation
{
    /// <summary>
    /// Aggregates global metrics from the world for observation or logging.
    /// </summary>
    public class MetricsCollector
    {
        public float TotalFood { get; private set; }
        public float TotalWater { get; private set; }
        public float TotalTimber { get; private set; }

        public void CollectMetrics(Grid grid, SimulationClock clock)
        {
            TotalFood = grid.TotalFood;
            TotalWater = grid.TotalWater;
            TotalTimber = grid.TotalTimber;
        }

        public void PrintMetrics(long tick)
        {
            System.Console.WriteLine($"Tick {tick}: Food={TotalFood:0.00}, Water={TotalWater:0.00}, Timber={TotalTimber:0.00}");
        }
    }
}
