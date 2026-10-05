# 10 - QA, UX & Accessibility

## Purpose

Treat usability, accessibility, stability and regression coverage as production requirements throughout Alpha rather than a final cleanup pass.

---

## QA layers

### 1. Unit tests

Use for deterministic rules, calculations, data transforms, event thresholds and save schema helpers.

### 2. Integration tests

Use for:

- create world → run → save → load → continue,
- selection across simulation updates,
- history generation,
- graph data pipelines,
- event detection,
- versioned save migration where supported.

### 3. Golden-seed regression

Use fixed seeds to detect unintended outcome changes.

### 4. Soak tests

Run long sessions to detect:

- memory growth,
- history bloat,
- performance degradation,
- event spam,
- save growth,
- rare crashes.

### 5. UX tests

Observe fresh users attempting concrete tasks with minimal instruction.

---

## Core smoke suite

Every release-candidate build should be able to pass a short smoke run covering:

1. launch,
2. new vivarium,
3. seed entry,
4. world creation,
5. pause/resume,
6. every speed mode,
7. camera pan/zoom,
8. select animal,
9. follow animal,
10. open population view,
11. open graph,
12. open history/timeline,
13. save,
14. return to menu,
15. load,
16. verify continued deterministic state,
17. exit cleanly.

This is the player-critical path.

---

## UX task testing

Use tasks rather than asking only whether the interface “feels good.”

Example tasks:

- Create seed `12345` and tell me what year it is.
- Find a wolf and follow it.
- Determine whether wolves are increasing or declining.
- Find the region with the lowest biomass.
- Tell me when the largest rabbit crash occurred.
- Find one living descendant of a selected animal.
- Save the world and reopen it.

Record:

- completion rate,
- completion time,
- wrong turns,
- UI elements overlooked,
- places where the tester asks for help,
- interpretation errors.

---

## Fresh-eyes testing

The developer knows too much about the simulation to judge discoverability reliably.

At milestone gates, use someone unfamiliar with the implementation.

Do not explain controls before the task unless onboarding itself is being tested.

The goal is to observe reality, not prove the design works.

---

## Accessibility baseline

### Text and UI scale

- scalable UI/text,
- avoid critical text baked into art,
- preserve layout at larger sizes,
- readable minimum font sizing.

### Colour

- do not encode critical meaning by hue alone,
- use shape/symbol/value differences,
- test common colour-vision deficiencies,
- provide sufficient contrast.

### Motion

- reduced-motion option for camera easing/visual interpolation where practical,
- avoid unnecessary flashing,
- ensure high-speed simulation does not create harmful flicker patterns.

### Input

- consistent keyboard/mouse behaviour,
- remapping architecture where feasible,
- avoid requiring rapid repeated actions,
- make pause immediately accessible.

### Information

- legends for overlays,
- clear graph labels,
- text equivalents for symbol-only critical states,
- readable selected/focused state.

---

## Visual readability tests

Test the final style in:

- grayscale,
- simulated colour-vision deficiencies,
- dense animal populations,
- dense forests,
- snowy/high-contrast biomes,
- low zoom,
- high zoom,
- overlays enabled,
- selected animal inside visually busy terrain.

If a biome/species only works because of colour, redesign its symbol language.

---

## Error-state UX

Test intentionally:

- unsupported save version,
- corrupted save,
- failed write,
- missing file,
- invalid seed entry,
- world creation failure,
- history data partially unavailable.

Errors should explain:

- what happened,
- whether data is safe,
- what the player can do next.

Avoid generic “Something went wrong” messages when a useful recovery action exists.

---

## Compatibility matrix

As target platforms become known, maintain a practical matrix across:

- supported OS versions,
- common display resolutions,
- windowed/fullscreen states,
- lower-spec target hardware,
- reference hardware,
- high-refresh displays where relevant.

Do not expand the matrix beyond platforms actually intended for the milestone.

---

## Bug database fields

Every significant bug should record:

- title,
- build/commit,
- severity,
- reproducibility,
- steps,
- expected result,
- actual result,
- seed/save if relevant,
- screenshot/video/log where useful,
- subsystem,
- regression/non-regression status.

Simulation bugs without the seed/version are often difficult to reproduce; capture them automatically where possible.

---

## Telemetry philosophy

For local development/opt-in testing, useful metrics may include:

- crash frequency,
- load/save duration,
- average session length,
- commonly used time speeds,
- camera zoom distribution,
- which observer tools are used,
- performance samples.

Do not collect data merely because it is possible. Instrument questions the team actually intends to answer and respect privacy/consent requirements for any real player telemetry.

---

## Phase-quality rule

A feature does not pass because its happy path works for the developer.

It must survive:

- automation,
- edge cases,
- fresh-user interaction,
- accessibility checks,
- release-like build testing,
- long-run integration with the simulation.