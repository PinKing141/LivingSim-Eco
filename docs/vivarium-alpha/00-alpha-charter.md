# 00 - Vivarium Alpha Charter

## Mission

Turn the proven LivingSim-Eco simulation into a compelling observer experience without compromising the autonomy, determinism, scale, or ecological integrity of the foundation.

Alpha is successful when the player can create a seeded world, understand what is happening, investigate why it happened, follow individuals and populations through time, and remain engaged without needing intervention mechanics.

---

## Player promise

The player is an **observer-naturalist**, not a manager or god.

They may:

- create or load a world,
- choose a seed,
- pause and change simulation speed,
- navigate and zoom,
- select and follow animals,
- inspect populations and traits,
- inspect graphs and overlays,
- investigate ancestry and descendants,
- review natural-history events and timelines.

They may not:

- spawn or delete animals,
- feed populations,
- change weather to rescue a species,
- force breeding,
- directly alter traits,
- paint resources into the world,
- prevent extinction through intervention.

Debug/developer tools may do these things, but they are not part of the player fantasy.

---

## Experience pillars

### 1. The world acts without you

The simulation continues to make meaningful decisions and produce consequences whether or not the player is watching a particular animal.

### 2. Every pattern should be investigable

The player should be able to move from:

`Something changed`

into:

`What changed?`

then:

`Why did it change?`

and finally:

`What happened afterward?`

### 3. Individuals create emotional hooks

The simulation may contain thousands of animals, but selecting one should create a comprehensible life story: birth, parents, movement, survival, reproduction, descendants and death.

### 4. Populations create deep-time stories

The player should be able to zoom out from one animal and understand centuries of ecological and evolutionary change.

### 5. Information is layered, not dumped

The default world view should remain readable and calm. Detailed data appears when requested through inspection, graphs, overlays and history tools.

### 6. Time is a primary interaction

The player should comfortably move between:

- individual moments,
- daily/seasonal observation,
- generational change,
- centuries of ecological history.

---

## Alpha non-goals

The following are deliberately not required for Alpha:

- a huge species roster,
- aquatic ecosystems,
- disease systems,
- wildfire systems,
- advanced speciation,
- final release content quantity,
- achievements,
- storefront integration,
- multiplayer,
- mod support,
- elaborate narrative scripting,
- player intervention mechanics,
- final marketing polish.

These may become later-roadmap work after the Alpha experience is proven.

---

## Quality bar

Every player-facing feature must satisfy four questions:

1. **Readable** — can the player understand it quickly?
2. **Useful** — does it help observe or explain the ecosystem?
3. **Cheap enough** — does it preserve simulation scale and fast-forward?
4. **Non-invasive** — does it avoid changing ecological outcomes merely because it is being observed?

If a feature fails one of these, it is not ready for Alpha.

---

## Decision hierarchy

When requirements conflict, use this priority order:

1. simulation correctness,
2. determinism/save integrity,
3. observability and comprehension,
4. performance and scalability,
5. input/camera responsiveness,
6. visual polish,
7. content quantity.

This prevents presentation work from corrupting the simulation or content growth from hiding systemic problems.

---

## Definition of Alpha complete

Alpha is not complete because every planned feature exists.

It is complete when external or fresh-eye testing demonstrates that players can:

- create a world without assistance,
- navigate it comfortably,
- identify notable ecological change,
- investigate at least one causal chain,
- follow an animal or population over time,
- use history/graphs to understand the past,
- distinguish important visual states,
- remain engaged in observation sessions without intervention mechanics,
- save, reload and continue deterministically,
- do all of the above at the agreed performance target.

The detailed proof is defined in `11-alpha-exit-gate.md`.