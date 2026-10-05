namespace LivingSim.Core;

/// <summary>Opt-in food-web accounting; does not affect simulation decisions.</summary>
public sealed class ResourceTelemetry
{
    private readonly Dictionary<AnimalSpecies, long> _plantConsumptionBySpecies = [];
    public IReadOnlyDictionary<AnimalSpecies, long> PlantConsumptionBySpecies => _plantConsumptionBySpecies;
    public long PlantConsumed { get; private set; }
    public long PlantRegenerated { get; private set; }
    public int HerbivoreStarvationDeaths { get; private set; }

    internal void RecordConsumption(AnimalSpecies species, int amount)
    {
        PlantConsumed += amount;
        _plantConsumptionBySpecies.TryGetValue(species, out var previous);
        _plantConsumptionBySpecies[species] = previous + amount;
    }
    internal void RecordRegeneration(int amount) => PlantRegenerated += amount;
    internal void RecordHerbivoreStarvation() => HerbivoreStarvationDeaths++;
}
