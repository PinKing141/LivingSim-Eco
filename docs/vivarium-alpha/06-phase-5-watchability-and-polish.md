# 06 - Phase 5: Watchability & Experience Polish

## Goal

Make LivingSim comfortable and compelling to watch for long sessions.

This phase does not add major ecological systems. It improves presentation, pacing, feedback, and emotional continuity around the simulation that already exists.

---

## Core outcome

The player should be able to spend long periods doing nothing more than:

- watching,
- following,
- speeding time up and down,
- checking a graph,
- opening a lineage,
- noticing a world change,
- returning to observation.

The experience should remain calm, legible and responsive.

---

## Movement presentation

Simulation remains discrete and deterministic; presentation may interpolate.

Requirements:

- smooth entity motion between simulation positions where appropriate,
- no visual teleporting during normal-speed observation,
- clear behaviour during extreme fast-forward,
- interpolation never feeds back into simulation state,
- selection marker follows the visual entity correctly,
- dead/despawned entities resolve cleanly.

At high time scales, favour clarity over pretending every movement can be shown.

---

## Time-compression presentation

Each speed mode should have its own presentation policy.

### Normal observation

- smooth movement,
- full ambient feedback,
- individual behaviour readable.

### Accelerated

- reduced animation detail if needed,
- population movement still readable.

### Fast-forward

- batch or sample presentation updates,
- emphasise population/environment trends,
- avoid unreadable visual flicker.

### Extreme fast-forward

- simulation throughput takes priority,
- renderer may update at a much lower rate,
- UI should communicate years/decades advancing clearly.

---

## Environmental feedback

Make large-scale change visible without requiring a graph.

Examples:

- seasonal ground/vegetation shift,
- snow cover,
- drought dryness,
- biomass depletion/recovery,
- water/climate state where simulated,
- local density change,
- carcass/mortality clustering.

The player should be able to notice that a region is under ecological stress before opening a data panel.

---

## Information pacing

Do not constantly interrupt the player with events.

Use a hierarchy:

- passive world change,
- subtle timeline/event indicator,
- important event notification,
- critical extinction/world-history event.

Player controls should include notification filters and quiet observation.

---

## Emotional continuity

Support attachment without turning the game into scripted narrative.

Potential features:

- pin/favourite an animal,
- pin a population,
- follow descendants,
- recent-life summary on death,
- “where are they now?” descendant shortcut,
- persistent notable lineage markers.

These are observation conveniences, not gameplay bonuses.

---

## Audio direction

Audio is optional for early Alpha but should be considered as a watchability layer.

Possible goals:

- unobtrusive ambient biome beds,
- seasonal/environmental variation,
- restrained animal/event cues,
- UI sounds that communicate state without fatigue.

Avoid constant reactive sound spam from thousands of agents.

---

## Readability under load

Test watchability in difficult scenes:

- thousands of animals,
- dense forests,
- overlapping species,
- multiple overlays,
- migration waves,
- mass mortality,
- snow or high-contrast terrain,
- extreme fast-forward.

Introduce level-of-detail rules for visual information rather than trying to draw every detail at every zoom.

---

## Long-session UX testing

Run observation sessions of at least:

- 10 minutes,
- 30 minutes,
- 60 minutes.

Ask testers to narrate:

- what they noticed,
- what they cared about,
- what confused them,
- what made them speed up/slow down,
- whether they formed attachment to an animal/population,
- whether they could explain a major ecological event.

Track when they become bored and what was happening immediately beforehand.

---

## Polish priorities

Prioritise in this order:

1. input/camera responsiveness,
2. clarity of world state,
3. time-control feedback,
4. selection/follow reliability,
5. event/history comprehension,
6. motion quality,
7. visual effects,
8. decorative polish.

Do not polish decorative details while core observation remains frustrating.

---

## Phase exit gate

Phase 5 passes when:

- normal-speed watching feels smooth,
- fast-forward remains readable,
- large environmental changes are visible without opening graphs,
- event notifications do not overwhelm the player,
- testers can comfortably follow individuals and populations,
- at least some testers form spontaneous attachment or curiosity around simulated entities,
- 30–60 minute observation sessions remain understandable and stable,
- no polish system changes deterministic simulation outcomes.

Once this passes, run the full Alpha Exit Gate.