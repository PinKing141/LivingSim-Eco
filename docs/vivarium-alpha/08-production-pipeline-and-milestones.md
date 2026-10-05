# 08 - Production Pipeline & Milestone Discipline

## Purpose

Apply professional game-production practices to Vivarium Alpha without importing unnecessary AAA bureaucracy.

The objective is to make progress measurable, reduce rework, catch risk early, and prevent half-finished systems from piling up.

---

## Milestone model

Every Alpha phase should pass through four states:

### 1. Definition

Before implementation begins:

- goal is written,
- non-goals are written,
- dependencies are known,
- technical risks are listed,
- player-facing acceptance criteria are defined,
- test plan exists,
- performance/storage impact is estimated.

### 2. Prototype

Build the smallest version that proves the risky assumption.

Prototype code may be temporary, but the experiment must answer a specific question.

### 3. Production implementation

Once the prototype proves the approach:

- build the maintainable version,
- integrate with save/load,
- add automation,
- add accessibility hooks,
- instrument performance,
- document behaviour.

### 4. Exit gate

A feature is not complete because it compiles or looks correct once.

It is complete only when its phase exit criteria, regression tests, performance budget and UX checks pass.

---

## Vertical-slice discipline

For complex features, build one full thin path before broad implementation.

Example for history:

`one animal → ancestry → timeline event → graph link → map focus → save/reload`

Prove that complete chain before adding every event type and every lineage view.

This prevents producing large amounts of disconnected infrastructure.

---

## Definition of Ready

A task is ready to implement when:

- user/player value is understood,
- expected result is concrete,
- dependencies are complete or intentionally mocked,
- edge cases are identified,
- acceptance criteria are testable,
- likely performance risks are known,
- there is no unresolved design decision that would invalidate the implementation.

If these are missing, the task belongs in design/prototyping, not production.

---

## Definition of Done

A player-facing feature is done only when:

- implementation is complete,
- automated tests cover critical logic,
- error states are handled,
- save/load implications are addressed,
- determinism is unaffected or intentionally versioned,
- profiling has been performed,
- keyboard/mouse/input flow is usable,
- accessibility requirements are considered,
- documentation is updated,
- no Severity 1 or Severity 2 bug remains in the feature,
- feature passes its acceptance criteria in a release-like build.

---

## Source-control discipline

Recommended flow:

- `main` stays buildable,
- feature branches are short-lived,
- each branch has one coherent purpose,
- integrate frequently,
- avoid giant multi-system branches,
- require tests before merge,
- record intentional simulation-rule changes explicitly.

For risky systems, prefer feature flags over long-lived divergence.

---

## Build pipeline

Maintain at least:

### Developer build

Fast iteration, assertions, debug tools, instrumentation.

### Profile build

Optimised enough for meaningful CPU/GPU/memory captures while retaining profiling markers.

### Release-candidate build

Represents what a player would receive.

Automate where practical:

- compile,
- unit/integration tests,
- deterministic seed smoke test,
- save/load smoke test,
- short performance benchmark,
- artifact output.

A broken automated build blocks new feature work until restored.

---

## Nightly/soak concept

If CI/runtime allows, maintain scheduled long-running checks:

- benchmark seed soak,
- save/reload continuation,
- high-population performance,
- long-history storage growth,
- event-generation spam detection,
- memory growth/leak detection.

Long simulations catch failures short tests cannot.

---

## Bug severity

### Severity 1 — blocker

Examples:

- save corruption,
- crash on common path,
- deterministic divergence,
- world cannot load,
- simulation fundamentally stops.

No milestone can pass with Severity 1 bugs.

### Severity 2 — major

Examples:

- important observation feature unusable,
- camera traps player,
- major graph/timeline misinformation,
- severe performance regression,
- inaccessible core workflow.

No phase exit with unresolved Severity 2 bugs unless explicitly waived and documented.

### Severity 3 — normal

Incorrect or frustrating behaviour with workaround.

### Severity 4 — polish

Minor visual, wording or low-impact consistency issue.

---

## Bug triage rule

Triage by:

`severity × frequency × player impact × regression risk`

Do not prioritise merely because a bug is visually obvious.

---

## Milestone reviews

At the end of each phase produce a short written review:

- what was planned,
- what shipped,
- what was cut,
- test results,
- performance deltas,
- known debt,
- risks carried forward,
- whether the phase gate passed.

This creates a development history and stops forgotten assumptions from becoming hidden dependencies.

---

## Feature cut policy

If a feature repeatedly misses quality/performance targets, choose one:

- simplify,
- defer,
- replace,
- cut.

Do not keep a feature merely because implementation time has already been spent on it.

Sunk cost is not a design requirement.

---

## Content lock / code lock concepts

Near the Alpha Exit Gate:

### Feature lock

No new major features. Only completion, fixes and polish.

### Content lock

No broad new species/biomes/systems.

### Code stabilisation

High-risk refactors require explicit justification.

This gives the Alpha build time to become trustworthy before evaluation.

---

## Production success criterion

The pipeline is working when:

- `main` remains usable,
- phases finish instead of accumulating,
- regressions are caught close to introduction,
- performance changes are visible,
- decisions are documented,
- scope can be reduced without destabilising the roadmap,
- the final Alpha build is the result of controlled convergence rather than a last-minute integration push.