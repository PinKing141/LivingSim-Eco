using System.Text.Json;

namespace LivingSim.Core;

public static class SimulationSaveService
{
    public const int CurrentFormatVersion = 1;

    private static readonly JsonSerializerOptions Options = new()
    {
        IncludeFields = true,
        WriteIndented = true,
    };

    public static void Save(WorldSimulation simulation, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var fullPath = Path.GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directory)) throw new InvalidOperationException("Save path must include a directory.");
        Directory.CreateDirectory(directory);

        var snapshot = new SimulationSave
        {
            FormatVersion = CurrentFormatVersion,
            SimulationVersion = SimulationVersion.Value,
            CreatedUtc = DateTimeOffset.UtcNow,
            Settings = simulation.World.Settings,
            Tick = simulation.Tick,
            Climate = simulation.Climate,
            Cells = simulation.World.Cells.ToArray(),
            Animals = simulation.Entities.Items.ToArray(),
            Carcasses = simulation.Carcasses.Items.ToArray(),
            ClimateHistory = simulation.ClimateHistory.Records.ToArray(),
            LineageRecords = simulation.Lineages.Records.Values.OrderBy(record => record.AnimalId).ToArray(),
            NaturalHistory = simulation.NaturalHistory.Snapshot(),
        };

        var temporaryPath = fullPath + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(snapshot, Options));
        File.Move(temporaryPath, fullPath, overwrite: true);
    }

    public static WorldSimulation Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var fullPath = Path.GetFullPath(path);
        var snapshot = JsonSerializer.Deserialize<SimulationSave>(File.ReadAllText(fullPath), Options)
            ?? throw new InvalidDataException("The save file is empty or invalid.");
        if (snapshot.FormatVersion != CurrentFormatVersion)
            throw new NotSupportedException($"Save format {snapshot.FormatVersion} is not supported by this build.");
        if (!string.Equals(snapshot.SimulationVersion, SimulationVersion.Value, StringComparison.Ordinal))
            throw new NotSupportedException($"Save was created by simulation {snapshot.SimulationVersion}; this build runs {SimulationVersion.Value}.");
        snapshot.Settings.Validate();
        if (snapshot.Tick < 0 || snapshot.Cells.Length != snapshot.Settings.Width * snapshot.Settings.Height)
            throw new InvalidDataException("The save's world dimensions or tick are invalid.");

        var simulation = new WorldSimulation(new World(snapshot.Settings, snapshot.Cells));
        simulation.Restore(snapshot.Tick, snapshot.Climate, snapshot.Animals, snapshot.Carcasses, snapshot.ClimateHistory, snapshot.LineageRecords, snapshot.NaturalHistory);
        return simulation;
    }
}
