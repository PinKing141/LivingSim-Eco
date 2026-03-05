namespace LivingSim.Config
{
    public sealed class SimulationConfig
    {
        public int RandomSeed { get; init; } = 12345;
        public int WorldWidth { get; init; } = 30;
        public int WorldHeight { get; init; } = 15;
        public int WorldGenerationSteps { get; init; } = 20;

        public int InitialCarnivores { get; init; } = 3;
        public int InitialHerbivores { get; init; } = 12;
        public int InitialOmnivores { get; init; } = 8;

        public int SimulationTicks { get; init; } = 2000;
        public int InitialSimulationDelayMs { get; init; } = 100;
        public int PausedDelayMs { get; init; } = 100;
        public int DelayAdjustmentStepMs { get; init; } = 10;

        public int PopulationHistoryLength { get; init; } = 60;
        public bool RenderMetricsToConsole { get; init; } = true;
    }
}
