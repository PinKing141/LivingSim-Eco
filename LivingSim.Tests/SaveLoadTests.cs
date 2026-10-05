using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class SaveLoadTests
{
    [Fact]
    public void SaveLoad_PreservesStateAndDeterministicContinuation()
    {
        var original = HeadlessWorldRunner.CreateFoodWeb(new WorldSettings(321, 40, 30),
            new SpeciesPopulation(AnimalSpecies.Herbivore, 18),
            new SpeciesPopulation(AnimalSpecies.Predator, 3),
            new SpeciesPopulation(AnimalSpecies.Omnivore, 3),
            new SpeciesPopulation(AnimalSpecies.Scavenger, 2));
        original.Advance(400);
        var path = Path.Combine(Path.GetTempPath(), $"livingsim-{Guid.NewGuid():N}.json");
        try
        {
            SimulationSaveService.Save(original, path);
            var loaded = SimulationSaveService.Load(path);

            Assert.Equal(SimulationVersion.Value, ReadVersion(path));
            Assert.Equal(SimulationStateHasher.Hash(original), SimulationStateHasher.Hash(loaded));
            Assert.Equal(original.Lineages.Records.Count, loaded.Lineages.Records.Count);
            Assert.Equal(original.NaturalHistory.Events.Count, loaded.NaturalHistory.Events.Count);

            original.Advance(500);
            loaded.Advance(500);

            Assert.Equal(SimulationStateHasher.Hash(original), SimulationStateHasher.Hash(loaded));
            Assert.Equal(original.NaturalHistory.Events, loaded.NaturalHistory.Events);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string ReadVersion(string path)
    {
        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.GetProperty("SimulationVersion").GetString()!;
    }
}
