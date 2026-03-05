using LivingSim.World;
using LivingSim.Core;

namespace LivingSim.Observation
{
    /// <summary>
    /// Aggregates global metrics from the world for observation or logging.
    /// </summary>
    public class MetricsCollector
    {
        public MetricsSnapshot CurrentSnapshot { get; private set; }

        public event Action<MetricsSnapshot>? SnapshotCollected;

        public MetricsSnapshot CollectMetrics(Grid grid, SimulationClock clock)
        {
            var snapshot = new MetricsSnapshot(
                Tick: clock.CurrentTick,
                TotalFood: grid.TotalFood,
                TotalWater: grid.TotalWater,
                TotalTimber: grid.TotalTimber);

            CurrentSnapshot = snapshot;
            SnapshotCollected?.Invoke(snapshot);
            return snapshot;
        }
    }
}
