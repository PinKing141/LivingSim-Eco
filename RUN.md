# Run LivingSim

The current build is Slice 2: a headless deterministic predator/prey survival loop.

```powershell
dotnet run --project LivingSim.csproj -- --seed 10 --width 96 --height 64 --ticks 500 --herbivores 24 --predators 3
```

The command prints the simulation version, input settings, completed tick count, living herbivore/predator counts, carcass count, and a deterministic state hash. Re-running it with the same options should return the same output.

Run the automated determinism and headless-core checks with:

```powershell
dotnet test LivingSim.sln
```
