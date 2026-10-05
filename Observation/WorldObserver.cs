using System.Text;
using LivingSim.Core;

namespace LivingSim.Observation;

public static class WorldObserver
{
    public static (int Width, int Height) ViewportFor(ObserverState state) => state.ZoomLevel switch
    {
        0 => (72, 28),
        2 => (32, 14),
        3 => (20, 10),
        _ => (48, 20),
    };

    public static string Render(WorldSimulation simulation, ObserverState state, ObserverController? controller = null, int? viewportWidth = null, int? viewportHeight = null)
    {
        var viewport = ViewportFor(state);
        var requestedWidth = viewportWidth ?? viewport.Width;
        var requestedHeight = viewportHeight ?? viewport.Height;
        var cameraX = controller?.RenderCameraX ?? state.CameraX;
        var cameraY = controller?.RenderCameraY ?? state.CameraY;
        var startX = Math.Clamp(cameraX - requestedWidth / 2, 0, Math.Max(0, simulation.World.Width - requestedWidth));
        var startY = Math.Clamp(cameraY - requestedHeight / 2, 0, Math.Max(0, simulation.World.Height - requestedHeight));
        var width = Math.Min(requestedWidth, simulation.World.Width);
        var height = Math.Min(requestedHeight, simulation.World.Height);
        var characters = new char[height, width];
        var density = new int[height, width];

        foreach (var animal in simulation.Entities.Items)
        {
            var position = controller?.RenderedPosition(animal) ?? (animal.X, animal.Y);
            if (animal.IsAlive && position.X >= startX && position.X < startX + width && position.Y >= startY && position.Y < startY + height)
            {
                density[position.Y - startY, position.X - startX]++;
            }
        }

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                characters[y, x] = state.Overlay == ObservationOverlay.PopulationDensity
                    ? Gradient(density[y, x], 4)
                    : CellGlyph(simulation.World.CellAt(startX + x, startY + y), state.Overlay, simulation.Climate);
            }
        }

        foreach (var animal in simulation.Entities.Items)
        {
            var position = controller?.RenderedPosition(animal) ?? (animal.X, animal.Y);
            if (!animal.IsAlive || position.X < startX || position.X >= startX + width || position.Y < startY || position.Y >= startY + height)
            {
                continue;
            }

            characters[position.Y - startY, position.X - startX] = state.SelectedAnimalId == animal.Id ? '@' : AnimalGlyph(animal.Species);
        }

        var output = new StringBuilder();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++) output.Append(characters[y, x]);
            output.AppendLine();
        }

        return output.ToString();
    }

    public static string Inspector(WorldSimulation simulation, ObserverState state)
    {
        var summary = $"Tick {simulation.Tick} | {simulation.Climate.Season} | herbivores {simulation.Metrics.Herbivores} | predators {simulation.Metrics.Predators} | carcasses {simulation.Metrics.Carcasses}";
        if (!state.SelectedAnimalId.HasValue || state.SelectedAnimalId.Value > simulation.Entities.Count)
        {
            return summary + Environment.NewLine + "No animal selected.";
        }

        var animal = simulation.Entities.GetById(state.SelectedAnimalId.Value);
        var ancestors = simulation.Lineages.AncestorsOf(animal.Id).Count;
        var descendants = simulation.Lineages.DescendantsOf(animal.Id).Count;
        return summary + Environment.NewLine +
            $"#{animal.Id} {animal.Species} {animal.Sex} alive={animal.IsAlive} pos=({animal.X},{animal.Y}) energy={animal.Energy} health={animal.Health} gen={animal.Generation} group={animal.GroupId}" + Environment.NewLine +
            $"traits: speed={animal.Traits.Speed} metabolism={animal.Traits.Metabolism} vision={animal.Traits.Vision} size={animal.Traits.Size} fertility={animal.Traits.Fertility}" + Environment.NewLine +
            $"lineage: parents=({animal.ParentAId},{animal.ParentBId}) ancestors={ancestors} descendants={descendants}";
    }

    public static string Graphs(IReadOnlyList<ObserverSample> history)
    {
        if (history.Count == 0) return "Graphs: collecting…";
        return $"Graph herbivores {Sparkline(history.Select(sample => (double)sample.Herbivores))}" + Environment.NewLine +
            $"Graph predators  {Sparkline(history.Select(sample => (double)sample.Predators))}" + Environment.NewLine +
            $"Graph speed      {Sparkline(history.Select(sample => sample.PlantEaterSpeed))}";
    }

    public static string Timeline(WorldSimulation simulation)
    {
        var events = simulation.NaturalHistory.Events.TakeLast(3).ToArray();
        return events.Length == 0
            ? "History: collecting notable events…"
            : "History: " + string.Join(" | ", events.Select(history => $"t{history.Tick} {history.Summary}"));
    }

    public static string PopulationPanel(WorldSimulation simulation) => simulation.PopulationBySpecies.Count == 0
        ? "Population: no living animals"
        : "Population: " + string.Join(" | ", simulation.PopulationBySpecies.OrderBy(entry => entry.Key).Select(entry => $"{AnimalGlyph(entry.Key)} {entry.Key} {entry.Value}"));

    private static char CellGlyph(WorldCell cell, ObservationOverlay overlay, ClimateState climate) => overlay switch
    {
        ObservationOverlay.Biomass => Gradient(cell.PlantBiomass, cell.MaxPlantBiomass),
        ObservationOverlay.HabitatPressure => Gradient(cell.HabitatPressure, 1_000),
        ObservationOverlay.Climate => climate.IsDrought ? 'd' : climate.IsAbundance ? 'a' : 'c',
        _ => cell.Terrain == TerrainType.Water ? '~' : cell.PlantBiomass > 0 ? '.' : '_',
    };

    private static char Gradient(int value, int maximum)
    {
        const string glyphs = " .:oO#";
        if (maximum <= 0) return glyphs[0];
        return glyphs[Math.Clamp(value * (glyphs.Length - 1) / maximum, 0, glyphs.Length - 1)];
    }

    private static char AnimalGlyph(AnimalSpecies species) => species switch
    {
        AnimalSpecies.Herbivore => 'h',
        AnimalSpecies.LargeHerbivore => 'H',
        AnimalSpecies.SmallHerbivore => 's',
        AnimalSpecies.Predator => 'p',
        AnimalSpecies.ApexPredator => 'P',
        AnimalSpecies.Omnivore => 'o',
        AnimalSpecies.Scavenger => 'v',
        _ => '?',
    };

    private static string Sparkline(IEnumerable<double> values)
    {
        const string glyphs = "▁▂▃▄▅▆▇█";
        var points = values.TakeLast(48).ToArray();
        var minimum = points.Min();
        var maximum = points.Max();
        if (maximum == minimum) return new string(glyphs[0], points.Length);
        return string.Concat(points.Select(value => glyphs[(int)Math.Clamp((value - minimum) * (glyphs.Length - 1) / (maximum - minimum), 0, glyphs.Length - 1)]));
    }
}
