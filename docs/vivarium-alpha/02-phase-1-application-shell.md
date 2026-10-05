# 02 - Phase 1: Application Shell

## Goal

Turn the headless simulation into a reliable application flow without changing ecological outcomes.

The player must be able to create a vivarium, enter or randomise a seed, save it, load it, pause it and control time with confidence.

---

## Player flow

Target golden path:

`Launch → New Vivarium → Seed → Create → Observe → Pause/Speed → Save → Exit → Load → Continue`

Every step should be understandable without external instructions.

---

## Core features

### New Vivarium flow

- seed text entry,
- random seed button,
- copy seed,
- deterministic confirmation that the same seed recreates the same initial world,
- world creation progress state if generation is not instantaneous,
- explicit simulation-version metadata stored with the world.

Avoid exposing dozens of generation sliders during Alpha. Seed is the primary world identity.

### Save/load shell

Provide:

- manual save,
- autosave policy,
- load list,
- world seed display,
- simulated age/year,
- last-played timestamp,
- save version/status,
- clear handling of incompatible or corrupted saves.

Never silently overwrite an incompatible save.

### Time controls

Support distinct levels rather than an uncontrolled continuous multiplier:

- paused,
- normal observation speed,
- accelerated observation,
- fast-forward,
- extreme/headless-style fast-forward where presentation may intentionally reduce update frequency.

Simulation time and render time must remain separate.

### Session safety

- clean shutdown,
- no world mutation after save completion unless simulation resumes,
- guard against double-save/race conditions,
- clear feedback during long save/load operations,
- crash-recovery strategy where practical.

---

## Engineering requirements

### Simulation/presentation separation

Application code may command the simulation to:

- advance,
- pause,
- serialise,
- expose read-only observation state.

Presentation must not become a hidden input to simulation outcomes.

### State machine

Use an explicit application/session state model such as:

- Boot,
- Main Menu,
- Creating World,
- Loading,
- Running,
- Paused,
- Saving,
- Error/Recovery.

Avoid scattered booleans that permit impossible combinations.

### Feature flags

Player-facing Alpha features that are not ready should be gated behind explicit development flags rather than partially exposed.

### Failure handling

Expected failure states must be designed, not treated as edge cases:

- invalid seed input,
- missing save,
- unsupported save version,
- corrupted save,
- insufficient write permission,
- disk-write failure,
- interrupted generation.

---

## UX quality bar

A first-time tester should be able to:

- create a new world in under one minute,
- understand which seed is being used,
- identify pause and speed controls immediately,
- save and reload without instruction,
- understand whether a save is compatible.

Avoid modal spam. Reserve interruption for destructive or unrecoverable actions.

---

## Test matrix

### Functional

- same seed creates same initial world,
- random seed creates a valid seed,
- pause prevents simulation advancement,
- every speed mode advances at the intended cadence,
- save/load preserves canonical state,
- autosave cannot corrupt a manual save,
- incompatible saves are rejected safely.

### Stress

- repeated save/load cycles,
- rapid pause/unpause,
- switching speed modes repeatedly,
- loading the largest benchmark save,
- saving during high-population conditions,
- long session followed by save and reload.

### UX smoke test

Give a fresh tester only the instruction:

> Create a world with seed 12345, run it for a while, save it, quit, then reopen it.

Record where they hesitate.

---

## Performance budgets

Exact numbers may be adjusted after profiling, but budgets must exist before polish.

Track:

- cold launch time,
- world generation time,
- save time,
- load time,
- menu/input response latency,
- memory while sitting paused,
- extreme-fast-forward throughput with rendering reduced.

Any regression beyond the agreed threshold blocks the phase gate.

---

## Phase exit gate

Phase 1 passes when:

- the golden path works end-to-end,
- save/load remains deterministic,
- application states cannot produce corrupt simulation state,
- time controls are reliable,
- a fresh tester can create and resume a world without coaching,
- automated regression covers the critical flow,
- performance remains within the foundation budget.

Only then begin Phase 2.