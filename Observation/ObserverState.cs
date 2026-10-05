namespace LivingSim.Observation;

public sealed class ObserverState
{
    public bool IsPaused { get; private set; }
    public int TicksPerFrame { get; private set; } = 1;
    public int CameraX { get; private set; }
    public int CameraY { get; private set; }
    public int? SelectedAnimalId { get; private set; }
    public int? FollowAnimalId { get; private set; }
    public ObservationOverlay Overlay { get; private set; }
    public int ZoomLevel { get; private set; } = 1;

    public void TogglePause() => IsPaused = !IsPaused;
    public void SetTicksPerFrame(int ticks) => TicksPerFrame = Math.Clamp(ticks, 1, 10_000);
    public void MoveCamera(int deltaX, int deltaY) { CameraX += deltaX; CameraY += deltaY; }
    public void SetCamera(int x, int y) { CameraX = x; CameraY = y; }
    public void Select(int? animalId) => SelectedAnimalId = animalId;
    public void Follow(int? animalId) { FollowAnimalId = animalId; SelectedAnimalId = animalId; }
    public void SetOverlay(ObservationOverlay overlay) => Overlay = overlay;
    public void ZoomIn() => ZoomLevel = Math.Min(3, ZoomLevel + 1);
    public void ZoomOut() => ZoomLevel = Math.Max(0, ZoomLevel - 1);

    public void CycleOverlay() => Overlay = (ObservationOverlay)(((int)Overlay + 1) % Enum.GetValues<ObservationOverlay>().Length);
}
