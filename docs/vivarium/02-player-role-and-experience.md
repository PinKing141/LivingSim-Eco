# 02 - Player Role & Experience

## Player role

The default player role is the **Observer**.

The game can provide rich tools for viewing, following, comparing, and understanding the ecosystem without giving the player tools that directly alter ecological outcomes.

## World creation

The simplest complete flow should be:

1. Enter or randomise a seed.
2. Generate the world.
3. Start the simulation.
4. Observe.

Optional world parameters may be added later, but **seed-only generation must remain a valid way to play**.

When simulation rules and game version are identical, the same seed should reproduce the same generated starting state and deterministic simulation outcome.

## Allowed observational controls

The player may use:

- pause and resume,
- simulation speed controls,
- deep-time fast-forward,
- camera pan and zoom,
- follow-animal mode,
- animal inspection,
- species, population, group, and lineage inspection,
- population graphs,
- climate graphs,
- genetic-trait graphs,
- resource and population-density overlays,
- biome, territory, or migration overlays if those systems exist,
- timeline and event-history navigation.

These controls help the player **understand** the world rather than control it.

## Intervention boundary

The baseline observer mode should not allow the player to:

- spawn animals,
- kill animals,
- feed populations,
- heal individuals,
- alter climate,
- remove predators,
- directly force reproduction,
- repair ecological collapse.

Extinction and collapse are valid simulation outcomes, not failure states the player must correct.

If intervention mechanics are ever added, they should be clearly separated from the core observer experience.

## Core experience loop

The intended loop is:

**Generate -> Observe -> Notice -> Investigate -> Follow -> Fast-forward -> Discover**

There does not need to be a conventional win condition.

The reward is discovering what happened in a particular world and understanding the ecological chain of cause and effect behind it.

## Observation scales

### Individual scale

The player can follow one animal through its life and inspect things such as:

- age,
- health,
- needs,
- inherited traits,
- parents,
- offspring,
- group membership,
- optional lifetime statistics such as hunts or surviving offspring.

### Population scale

The player can observe:

- population booms and crashes,
- predator-prey oscillations,
- migration and range shifts,
- local extinction,
- recolonisation,
- populations adapting differently after geographic separation.

### Deep-time scale

The player can observe:

- trait averages changing over hundreds of generations,
- lineages rising and disappearing,
- drought and abundance changing selection pressure,
- ecological recovery after major crashes,
- world histories becoming recognisably unique to a seed.
