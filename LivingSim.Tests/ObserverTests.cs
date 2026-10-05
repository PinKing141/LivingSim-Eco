using LivingSim.Core;
using LivingSim.Observation;
using Xunit;

namespace LivingSim.Tests;

public sealed class ObserverTests
{
    [Fact]
    public void PauseAndSpeedControls_DoNotChangeCoreDeterminismRules()
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(Seed: 5, Width: 12, Height: 12), herbivores: 2, predators: 0);
        var observer = new ObserverController();

        observer.Advance(simulation);
        Assert.Equal(1, simulation.Tick);
        Assert.Single(observer.History);
        observer.State.TogglePause();
        observer.Advance(simulation);
        Assert.Equal(1, simulation.Tick);
        observer.State.TogglePause();
        observer.State.SetTicksPerFrame(5);
        observer.Advance(simulation);
        Assert.Equal(6, simulation.Tick);
    }

    [Fact]
    public void SelectionAndFollow_ExposeIndividualWithoutControllingIt()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 2, Width: 10, Height: 10));
        var animalId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 4, 6);
        var observer = new ObserverController();

        observer.SelectNext(simulation);
        observer.ToggleFollow();
        observer.State.TogglePause();
        observer.Advance(simulation);

        Assert.Equal(animalId, observer.State.SelectedAnimalId);
        Assert.Equal(animalId, observer.State.FollowAnimalId);
        Assert.Equal(4, observer.State.CameraX);
        Assert.Equal(6, observer.State.CameraY);
        Assert.Contains("Herbivore", WorldObserver.Inspector(simulation, observer.State));
    }

    [Fact]
    public void Renderer_ProvidesWorldAndOverlayViews()
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(Seed: 1, Width: 12, Height: 8), herbivores: 2, predators: 0);
        simulation.Advance();
        var state = new ObserverState();
        state.SetOverlay(ObservationOverlay.PopulationDensity);

        var rendered = WorldObserver.Render(simulation, state, viewportWidth: 12, viewportHeight: 8);

        Assert.Equal(8, rendered.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length);
        Assert.NotEmpty(rendered.Trim());
    }

    [Fact]
    public void ZoomAndPresentationCamera_ImproveObserverNavigationOnly()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 3, Width: 80, Height: 48));
        var observer = new ObserverController();
        observer.State.SetCamera(40, 24);
        observer.State.ZoomIn();
        observer.State.ZoomIn();
        observer.Advance(simulation);

        Assert.Equal((20, 10), WorldObserver.ViewportFor(observer.State));
        Assert.Equal(40, observer.RenderCameraX);
        Assert.Equal(24, observer.RenderCameraY);
        Assert.Empty(simulation.Entities.Items.ToArray());
    }
}
