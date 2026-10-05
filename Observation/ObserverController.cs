using LivingSim.Core;

namespace LivingSim.Observation;

public sealed class ObserverController
{
    private readonly List<ObserverSample> _history = [];
    private readonly Dictionary<int, (int X, int Y)> _previousPositions = [];
    private bool _hasPresentationFrame;
    private double _renderCameraX;
    private double _renderCameraY;

    public ObserverState State { get; } = new();
    public IReadOnlyList<ObserverSample> History => _history;
    public int RenderCameraX => (int)Math.Round(_renderCameraX);
    public int RenderCameraY => (int)Math.Round(_renderCameraY);

    public void Advance(WorldSimulation simulation)
    {
        if (!State.IsPaused)
        {
            CapturePositions(simulation);
            simulation.Advance(State.TicksPerFrame);
            _history.Add(new ObserverSample(simulation.Tick, simulation.Metrics.Herbivores, simulation.Metrics.Predators, simulation.Metrics.HerbivoreTraits.Speed));
            if (_history.Count > 120) _history.RemoveAt(0);
        }

        FollowSelection(simulation);
        EaseCamera(simulation);
    }

    public void SelectNext(WorldSimulation simulation)
    {
        var alive = new List<int>();
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.IsAlive) alive.Add(animal.Id);
        }
        if (alive.Count == 0)
        {
            State.Select(null);
            return;
        }

        var currentIndex = State.SelectedAnimalId.HasValue ? alive.IndexOf(State.SelectedAnimalId.Value) : -1;
        State.Select(alive[(currentIndex + 1 + alive.Count) % alive.Count]);
    }

    public void ToggleFollow() => State.Follow(State.FollowAnimalId.HasValue ? null : State.SelectedAnimalId);

    public void Focus(NaturalHistoryEvent historyEvent) => State.SetCamera(historyEvent.X, historyEvent.Y);

    public (int X, int Y) RenderedPosition(AnimalState animal)
    {
        if (!_previousPositions.TryGetValue(animal.Id, out var previous)) return (animal.X, animal.Y);
        return ((previous.X + animal.X + 1) / 2, (previous.Y + animal.Y + 1) / 2);
    }

    private void FollowSelection(WorldSimulation simulation)
    {
        if (!State.FollowAnimalId.HasValue || State.FollowAnimalId.Value > simulation.Entities.Count)
        {
            return;
        }

        var animal = simulation.Entities.GetById(State.FollowAnimalId.Value);
        if (!animal.IsAlive)
        {
            State.Follow(null);
            return;
        }

        State.SetCamera(animal.X, animal.Y);
    }

    private void CapturePositions(WorldSimulation simulation)
    {
        _previousPositions.Clear();
        foreach (var animal in simulation.Entities.Items) _previousPositions[animal.Id] = (animal.X, animal.Y);
    }

    private void EaseCamera(WorldSimulation simulation)
    {
        if (!_hasPresentationFrame)
        {
            _renderCameraX = State.CameraX;
            _renderCameraY = State.CameraY;
            _hasPresentationFrame = true;
            return;
        }

        _renderCameraX += (State.CameraX - _renderCameraX) * 0.38;
        _renderCameraY += (State.CameraY - _renderCameraY) * 0.38;
        _renderCameraX = Math.Clamp(_renderCameraX, 0, simulation.World.Width - 1);
        _renderCameraY = Math.Clamp(_renderCameraY, 0, simulation.World.Height - 1);
    }
}
