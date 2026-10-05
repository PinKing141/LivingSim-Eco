# 01 - Foundation Freeze & Handoff

## Purpose

Create a formal handoff from the simulation-foundation project into the player-facing Vivarium Alpha project.

This stage is not feature development. It is a release-engineering and production-baseline milestone.

---

## Entry criteria

All Foundation Gate Recovery tests must pass, including:

- ecological viability,
- predator/prey recovery,
- resource feedback,
- multi-species behaviour,
- evolution,
- climate pressure,
- geographic divergence,
- long-run stability,
- cross-seed diversity,
- deterministic save/reload,
- performance and scale.

The final Foundation Exit Gate must be green.

---

## Required outputs

### 1. Version freeze

Create the immutable reference point:

`LivingSim-Eco v0.1 - Simulation Foundation`

Record:

- commit SHA,
- simulation version string,
- save schema version,
- benchmark date,
- toolchain/runtime version,
- platform used for performance capture.

### 2. Benchmark seed pack

Archive a fixed seed cohort representing:

- stable coexistence,
- predator extinction,
- prey crash/recovery,
- high population density,
- drought stress,
- strong evolutionary drift,
- geographic divergence,
- worst-case performance,
- long-lived ecosystem,
- save/reload continuation.

These seeds become permanent regression fixtures.

### 3. Golden state hashes

For selected seeds, record canonical hashes at agreed checkpoints such as:

- Year 1,
- Year 10,
- Year 50,
- Year 250,
- Year 1,000 where practical.

Golden hashes must only change after an intentional simulation-rule version change.

### 4. Performance baseline

Capture at minimum:

- animal count,
- tick count,
- total runtime,
- ticks/second,
- peak memory,
- allocation rate if measurable,
- save size,
- save time,
- load time,
- history storage growth.

Also capture at least one dense/worst-case seed rather than only an average world.

### 5. Rule baseline

Document the simulation rules that affect outcomes:

- metabolism,
- reproduction,
- hunting,
- movement,
- inheritance/mutation,
- climate pressure,
- resource regeneration,
- death/lifecycle,
- population/group rules.

The purpose is not to forbid future tuning. The purpose is to make future changes explicit and versioned.

---

## Change-control rule during Alpha

Once `v0.1` is frozen, simulation-rule changes require all of the following:

1. a documented reason tied to a player-facing problem, bug, or proven ecological defect,
2. a simulation version increment,
3. deterministic regression reruns,
4. benchmark comparison,
5. save-compatibility decision,
6. updated golden hashes if the change intentionally alters outcomes.

Do not casually tune ecology while implementing UI or rendering.

---

## Save compatibility policy

Every Alpha save should carry:

- simulation version,
- save schema version,
- seed,
- deterministic RNG state,
- world-generation version,
- content/species-definition version if relevant.

If an old save cannot be safely migrated, fail clearly and preserve the file rather than silently loading it into altered rules.

---

## Build configurations

Adopt at least three practical configurations:

### Development

- assertions enabled,
- debug overlays,
- verbose diagnostics,
- instrumentation,
- developer commands.

### Profile

- near-release optimisation,
- profiling markers,
- benchmark reporting,
- minimal debug overhead.

### Player/Shipping Candidate

- no developer intervention controls exposed,
- production logging policy,
- release-like settings.

This prevents debug-only behaviour from becoming part of the player experience.

---

## Exit criteria

The handoff passes when:

- `v0.1` exists as an immutable baseline,
- benchmark seeds are archived,
- golden hashes are reproducible,
- performance numbers are recorded,
- save/schema versions are explicit,
- simulation rules have a documented baseline,
- Alpha can proceed without uncertainty about what the foundation guarantees.

Only then begin Phase 1.