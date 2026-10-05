# 03 - Phase 2: Observation

## Goal

Make the living world easy to navigate, read and investigate without changing it.

This phase establishes the core moment-to-moment player interaction: **look, select, follow, compare, inspect, zoom out, zoom back in**.

---

## Core interaction loop

`Notice → Select → Inspect → Follow → Compare → Change time scale → Reframe`

The player should never need to fight the camera or decode the UI before they can understand an ecological event.

---

## Camera system

### Requirements

- smooth pan,
- zoom across ecosystem and individual scales,
- predictable focus point during zoom,
- follow selected entity,
- break-follow cleanly on manual movement,
- jump to selected population/event/location,
- return to previous focus where useful.

### Camera rules

- never allow camera motion to affect simulation,
- preserve selection through zoom where possible,
- clamp gracefully at world bounds,
- keep interpolation visual-only,
- provide reduced-motion options for fast camera transitions.

### Camera test cases

- follow a fast-moving predator,
- follow an animal crossing chunk boundaries,
- zoom from whole-world to individual and back,
- switch selected animals rapidly,
- use extreme fast-forward while following,
- recover camera after target death.

---

## Selection model

Selection should support:

- animal,
- population/species,
- region/tile,
- notable event target,
- graph/timeline cross-link where practical.

Selection must be visually obvious without obscuring the thing selected.

Provide:

- current selection identity,
- location,
- relevant current state,
- clear route to deeper information,
- clear deselect/back behaviour.

---

## Animal inspection

Minimum information:

- unique ID/name label,
- species,
- sex if simulated,
- age/life stage,
- health/energy state,
- current behaviour/intent,
- generation,
- core inheritable traits,
- parents if known,
- offspring count,
- population/group identity,
- location/region.

Do not expose internal implementation values that are meaningless to players unless developer mode is enabled.

---

## Population/species inspection

Provide at minimum:

- current population,
- historical high/low,
- births/deaths over selected period,
- death causes,
- average trait values,
- trait distribution where useful,
- geographic range/density,
- resource/prey relationships,
- current trend indicator based on real data, not prediction.

---

## Overlays

Initial overlay candidates:

- population density,
- species range,
- plant biomass/resources,
- temperature/climate pressure,
- water availability,
- recent deaths/carcasses,
- migration/movement density if meaningful.

### Overlay rules

- one primary analytical question per overlay,
- clear legend,
- colour plus shape/value encoding where possible,
- overlays must remain readable for colour-vision deficiencies,
- turning an overlay on/off must not change simulation state,
- do not stack so many overlays that the map becomes unreadable.

---

## Graphs

Minimum graph set:

- population over time,
- births/deaths,
- biomass/resources,
- climate values,
- selected trait average/distribution.

Graph requirements:

- scrub/hover selected period where practical,
- consistent time axis,
- compare species/populations,
- highlight notable events,
- jump from graph event to timeline/world where useful,
- avoid implying causation when data only shows correlation.

---

## Information hierarchy

Use progressive disclosure:

### Layer 1 — world view

Only what is needed to watch.

### Layer 2 — selection card

Immediate answer to “what is this?”

### Layer 3 — detail panel

Traits, population, relationships and current context.

### Layer 4 — analysis

Graphs, overlays, history and lineage.

This keeps the default vivarium calm instead of looking like a dashboard application.

---

## UX evaluation tasks

A tester should be able to complete these without coaching:

1. find a predator,
2. follow it for one in-game year,
3. determine whether its population is growing or shrinking,
4. find the main prey population,
5. display resource pressure in that region,
6. compare two population trends,
7. return to normal observation view.

Record:

- task completion,
- time to completion,
- misclicks,
- UI dead ends,
- controls the tester fails to discover.

---

## Performance requirements

Observation must not destroy the simulation budget.

Profile:

- rendering at high visible entity density,
- selection queries,
- follow mode,
- overlay generation,
- graph updates,
- world-level zoom,
- extreme fast-forward with UI open.

Expensive analysis may update at a lower presentation frequency than simulation ticks.

---

## Phase exit gate

Phase 2 passes when:

- camera/navigation is comfortable across all zoom scales,
- animals and populations can be selected reliably,
- follow mode survives normal edge cases,
- overlays answer distinct ecological questions,
- graphs expose meaningful trends,
- fresh testers can investigate a population without coaching,
- observation tools do not alter deterministic outcomes,
- performance remains inside agreed budgets.

Only then begin Phase 3.