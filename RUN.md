# Run LivingSim

The current build is Slice 11: a deterministic, versioned vivarium foundation with a polished terminal observer, lineage/history inspection, saves, and release benchmarks.

```powershell
dotnet run --project LivingSim.csproj -- --seed 10 --width 96 --height 64 --ticks 500 --herbivores 24 --predators 3
```

Optional food-web populations can be added with `--large-herbivores`, `--small-herbivores`, `--apex-predators`, `--omnivores`, and `--scavengers`.

Launch the interactive observer with `--observe`. It supports pause/resume, speed controls, eased camera/animal presentation, `Z`/`X` zoom, camera movement, biomass/habitat-pressure/population-density/climate overlays, live population and trait graphs, animal selection and follow mode. Selected animals show known ancestry/descendant counts; the timeline reports major population, extinction, migration, and trait events. Press `H` to focus the latest timeline event and `Esc` to exit.

Save a vivarium and resume it later:

```powershell
dotnet run --project LivingSim.csproj -- --seed 10 --ticks 500 --save .\vivariums\seed-10.json
dotnet run --project LivingSim.csproj -- --load .\vivariums\seed-10.json --ticks 500
```

Run a release benchmark with `--benchmark <density|coexistence|extinction|drought|evolution|migration|save-load> --ticks <count>`.

Run a sampled seed evaluation with `--foundation-gate --seed <seed> --width 64 --height 48 --ticks 24000 --herbivores 24 --predators 3`. The current [exit gate report](docs/vivarium/11-foundation-exit-gate.md) records a failed foundation decision.

The command prints the simulation version, input settings, completed tick count, living herbivore/predator counts, carcass count, herbivore trait averages, ecological samples/signals, group/migration counts, current climate, natural-history/lineage record counts, and a deterministic state hash. Re-running it with the same options should return the same output.

Run the automated determinism and headless-core checks with:

```powershell
dotnet test LivingSim.sln
```
