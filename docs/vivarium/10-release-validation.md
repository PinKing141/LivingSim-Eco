# Release validation

The first-release simulation scope is intentionally limited to seven ecological roles: medium, large, and small herbivores; predator and apex-predator; omnivore; and scavenger. Additional species or player-control systems are outside this foundation milestone.

## Benchmark catalog

Run a repeatable scenario with:

```powershell
dotnet run --project LivingSim.csproj -- --benchmark density --ticks 1000
```

Available scenarios are `density`, `coexistence`, `extinction`, `drought`, `evolution`, `migration`, and `save-load`. They cover the release benchmark categories without scripting outcomes.

## Persistence

Create a versioned save with `--save <path>`, then continue it with `--load <path> --ticks <count>`. A save contains the full world, living/dead entities, carcasses, lineage, climate/history state, and simulation-version metadata. Incompatible format or simulation versions are rejected rather than silently changing a vivarium's future.

## Automated checks

The test suite includes deterministic reload/continuation, a 1,000+ animal density run, and a 10,000-tick headless soak. Use `dotnet test LivingSim.sln` before release changes.
