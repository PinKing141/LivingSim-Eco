# 08 - Locked Decisions & Open Questions

## Decisions locked for now

These define the current concept and should be treated as the baseline unless the project vision changes.

### Core identity

- The project is an autonomous vivarium / ecosystem observer simulation.
- The player does not need direct ecosystem-management powers.
- The player is primarily an observer, investigator, and historian of the world.

### World creation

- Seed-driven generation is central.
- Seed-only world creation must remain a valid way to play.
- Determinism is important for testing, benchmarking, and seed sharing.

### Simulation

- Evolution and long-term ecological change are core features.
- The simulation should support meaningful deep time.
- The ecological core must run without graphics.
- Simulation and presentation remain separate.
- Observation tools are allowed and encouraged even though intervention is restricted.

### Architecture

- Fixed-tick simulation.
- Deterministic random behaviour.
- Stable simulation-system ordering.
- Headless execution as a first-class requirement.
- Spatial partitioning for local animal queries.
- Data-oriented design for large populations.

### Presentation

- **No final art style is locked.**
- **No rendering technology is locked.**
- ASCII/glyph rendering is a possible prototype direction, not a permanent requirement.
- The final renderer must not become a dependency of the simulation core.

## Intentionally unresolved questions

### Name

Possible current names include:

- LivingSim,
- Vivarium,
- LivingSim: Vivarium,
- another title discovered later.

The working documentation uses **LivingSim: Vivarium** only for clarity.

### Final visual style

Still open:

- ASCII/glyph-inspired,
- custom tile language,
- pixel art,
- vector-like abstraction,
- scientific visualisation,
- hybrid representation,
- another style discovered through prototyping.

### Rendering technology

Do not choose an engine/framework until presentation requirements are proven.

Potential lightweight graphical approaches may be investigated later, but none are architectural commitments today.

### World-generation controls

Open question:

Should the player only enter a seed, or should there also be optional advanced parameters such as:

- world size,
- climate profile,
- species set,
- resource abundance?

Whatever is added, seed-only generation should remain complete.

### First-release species roster

The exact species count is not decided.

The roadmap intentionally starts with two species and expands only when each new ecological role materially changes the simulation.

### Animal memory

The legacy prototype contained short-term memory, scents, dens, and territorial behaviour.

The rewrite does not assume those systems return.

Open question:

How much memory or social complexity is actually necessary to produce convincing ecological behaviour at scale?

### Social complexity

Potential future systems include:

- groups,
- packs,
- herds,
- flocking,
- territories,
- migration.

Complex leadership, relationship, or political systems are outside the baseline until proven necessary.

### Population / lineage definition

The game may identify geographically separated populations and important lineages for readability.

Open questions include:

- when does a population count as distinct,
- when does divergence become noteworthy,
- whether the game ever labels emergent variants as new species,
- how much genealogy can be stored at scale.

### Target scale

The architecture targets thousands of active animals, but final limits should be set through profiling rather than assumption.

Open values include:

- world dimensions,
- active entity target,
- headless tick throughput,
- simulation-history retention.

### Cross-version seed determinism

Strong requirement:

- the same version + same seed should reproduce the same world/history.

Open requirement:

- whether seeds must reproduce identical history after simulation rules change in later releases.

The likely approach is version-scoped determinism.

## Rejected-by-default feature test

When evaluating a new feature, ask:

1. Does it improve ecological simulation quality?
2. Does it make natural history more understandable?
3. Does it make the vivarium more enjoyable to watch?

If the answer to all three is no, the feature probably does not belong in the core game.

If the feature primarily gives the player control over nature, it should require especially strong justification or belong in a separate mode.
