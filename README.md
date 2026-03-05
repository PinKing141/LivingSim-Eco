# LivingSim-Eco

A console-based ecosystem simulation in C# where animals move, hunt, reproduce, and react to day/night + seasonal changes.

## Prerequisites
- .NET SDK 10.0+

## Run
```bash
dotnet run --project LivingSim.csproj
```

## Controls
- `↑` : speed up simulation (reduce delay)
- `↓` : slow down simulation (increase delay)
- `Space` : pause/resume
- `S` : toggle stats view

## Key configuration
Simulation tuning values are centralized in `Config/SimulationConfig.cs`, including:
- world size
- initial species counts
- generation steps
- simulation tick count
- delay/step timings
- population history window

## Project structure
- `Program.cs` — entrypoint + main loop wiring
- `Core/` — clock + world orchestration
- `World/` — grid, terrain, biome, scents, species
- `Animals/` — animal behavior and management
- `Environment/` — resource/weather tick systems
- `Generation/` — world generation/noise
- `Observation/` — metrics + logging
- `Visualisation/` — console renderer
