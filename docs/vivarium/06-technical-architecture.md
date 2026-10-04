# 06 - Technical Architecture

## Direction

The March 2026 redesign remains appropriate for the vivarium concept: a deterministic, data-oriented simulation core capable of running large populations independently of graphics.

The architecture should make it possible to run the ecosystem as:

- the interactive game,
- a headless simulation,
- an automated test,
- a benchmark,
- a long-running ecological experiment.

## Architectural rules

### Fixed simulation tick

Ecological simulation advances using a fixed discrete timestep.

The visual frame rate is independent.

No simulation rule should depend on frame rate or real-world elapsed rendering time.

### Deterministic randomness

Randomness must have explicit deterministic ownership.

Possible approaches include:

- one seeded world random source with a strict usage order,
- deterministic random streams owned by specific systems or entities.

The chosen model must support reproducible results.

### Stable system order

Simulation systems execute in a documented, stable order.

A possible early order is:

1. climate / season,
2. resource regeneration,
3. metabolism,
4. perception,
5. decision,
6. movement,
7. combat,
8. feeding,
9. reproduction,
10. lifecycle / death,
11. carcass processing,
12. spatial-index update,
13. metrics / history.

The exact list can evolve, but accidental order changes must not silently alter deterministic outcomes.

### Headless execution

The simulation must not require:

- a graphical window,
- UI,
- sound,
- console rendering,
- player input.

Headless execution is a first-class feature rather than a debugging afterthought.

### Spatial partitioning

Animals should query local space rather than scanning every entity globally.

The world may be divided into fixed chunks or buckets. Perception should normally inspect the current spatial partition and nearby partitions only.

This is essential for scaling from hundreds to thousands of animals.

### Data-oriented hot paths

The redesign should avoid unnecessary work inside the tick loop.

Target rules include:

- avoid LINQ in hot paths,
- avoid per-tick temporary collections where practical,
- reuse buffers,
- prefer compact contiguous data,
- avoid unnecessary object references between entities,
- profile before performing low-value micro-optimisation.

Determinism and correctness come before premature optimisation.

## Suggested module boundary

```text
LivingSim.Core
  World
  Climate
  Resources
  Entities / Components
  Genetics
  Simulation Systems
  Spatial Partitioning
  Metrics
  History

LivingSim.Presentation
  Renderer
  Camera
  Interpolation
  UI
  Graphs
  Overlays

LivingSim.App
  Startup
  Seed Entry
  Main Loop
  Save / Load
  Configuration
```

Names are illustrative; the dependency direction is more important than the exact folder structure.

## Dependency rule

`Core` must never depend on `Presentation`.

`Presentation` may read immutable or controlled snapshots/views from `Core`.

`App` composes the two.

This protects headless execution and makes the final rendering technology replaceable.

## Entity model

The previous prototype placed a large amount of state and behaviour inside individual `Animal` objects.

The rewrite should move toward entities as IDs plus data, with systems processing those data sets.

Candidate components/data groups include:

- transform / position,
- metabolism,
- lifecycle,
- health / combat,
- diet,
- species,
- genetics,
- reproductive state,
- group/population identity where retained.

Components should remain mostly data. Simulation rules belong in systems.

## Performance target

The exact release target should be determined by profiling, but the architecture should be designed with **thousands of concurrent animals** in mind.

Useful benchmark goals should eventually include:

- interactive rendering with large active populations,
- much faster-than-real-time headless execution,
- long soak tests across tens or hundreds of thousands of ticks,
- no severe garbage-collection spikes in steady-state simulation.

## Testing strategy

Determinism should be protected with tests from the beginning.

Tests should eventually cover:

- same seed -> same world,
- same seed + same inputs -> same simulation state after N ticks,
- fixed system execution order,
- reproduction rules,
- death and carcass rules,
- population accounting,
- save/load continuation,
- long-run invariant checks,
- benchmark seeds for known ecological behaviours.

## Save and simulation versioning

Saves should eventually store the simulation version that produced them.

This matters because changing a deterministic rule may legitimately change the future of a seed.

The game should be able to distinguish:

- seed identity,
- world-generation version,
- simulation-rules version,
- save-state version.

This makes reproducibility explicit rather than accidental.
