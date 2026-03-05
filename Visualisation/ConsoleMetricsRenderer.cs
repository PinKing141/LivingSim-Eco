using LivingSim.Observation;

namespace LivingSim.Visualisation
{
    /// <summary>
    /// Renders metrics snapshots to the console.
    /// </summary>
    public sealed class ConsoleMetricsRenderer
    {
        public string Format(MetricsSnapshot snapshot)
        {
            return $"Tick {snapshot.Tick}: Food={snapshot.TotalFood:0.00}, Water={snapshot.TotalWater:0.00}, Timber={snapshot.TotalTimber:0.00}";
        }

        public void Render(MetricsSnapshot snapshot)
        {
            Console.WriteLine(Format(snapshot));
        }
    }
}
