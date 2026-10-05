namespace LivingSim.Core;

public sealed class LongRunReport
{
    private readonly List<PopulationSample> _samples = [];

    public IReadOnlyList<PopulationSample> Samples => _samples;
    public EcologicalSignal Signals { get; private set; }

    public void Record(WorldSimulation simulation)
    {
        long biomass = 0;
        long pressure = 0;
        foreach (var cell in simulation.World.Cells)
        {
            biomass += cell.PlantBiomass;
            pressure += cell.HabitatPressure;
        }

        _samples.Add(new PopulationSample(
            simulation.Tick,
            simulation.Metrics.Herbivores,
            simulation.Metrics.Predators,
            biomass,
            simulation.World.CellCount == 0 ? 0 : (double)pressure / simulation.World.CellCount));
        Signals = Analyze();
    }

    private EcologicalSignal Analyze()
    {
        if (_samples.Count == 0)
        {
            return EcologicalSignal.None;
        }

        var result = EcologicalSignal.None;
        var latest = _samples[^1];
        if (latest.Herbivores == 0) result |= EcologicalSignal.HerbivoreExtinction;
        if (latest.Predators == 0) result |= EcologicalSignal.PredatorExtinction;
        if (_samples.Count >= 3 && _samples.TakeLast(3).All(sample => sample.PlantBiomass == 0)) result |= EcologicalSignal.BiomassDepleted;

        var initialPopulation = _samples[0].Herbivores + _samples[0].Predators;
        var largestPopulation = _samples.Max(sample => sample.Herbivores + sample.Predators);
        if (initialPopulation > 0 && largestPopulation > initialPopulation * 10) result |= EcologicalSignal.RunawayPopulation;

        if (_samples.Count >= 5)
        {
            var recent = _samples.TakeLast(5).Select(sample => sample.Herbivores + sample.Predators).ToArray();
            if (recent.Max() - recent.Min() <= 1) result |= EcologicalSignal.StaticPopulation;
        }

        return result;
    }
}
