# Run LivingSim

The current build is Slice 1: a headless deterministic world foundation with no animals yet.

```powershell
dotnet run --project LivingSim.csproj -- --seed 123 --width 96 --height 64 --ticks 10000
```

The command prints the simulation version, input settings, completed tick count, and a deterministic state hash. Re-running it with the same options should return the same hash.

Run the automated determinism and headless-core checks with:

```powershell
dotnet test LivingSim.sln
```
