# 05 - Presentation Direction

## Status

**No final art style or rendering technology is locked.**

The current ASCII/glyph discussion is a possible direction, not canon.

The project should first prove the simulation and observer experience before committing to a final visual identity.

## Presentation principle that is locked

Simulation state and rendering must remain separate.

The simulation exposes data such as:

- terrain,
- resources,
- animals,
- populations,
- traits,
- climate,
- history.

A presentation layer decides how those things are shown.

The simulation must remain able to run headless without a renderer.

## Possible presentation route

One possible direction is to move out of the operating-system terminal into a normal graphical window while retaining some of the readability and abstraction of ASCII.

This could include:

- glyphs rendered as tiles or sprites rather than terminal characters,
- custom symbols rather than literal keyboard glyphs,
- foreground/background colouring,
- layered terrain and animal rendering,
- smooth interpolation between fixed simulation positions,
- zoom-dependent representations,
- overlays and inspection panels,
- proper graphs and timeline UI.

This is a prototype direction only.

## Other valid directions

The final style could instead become:

- minimalist vector-like symbols,
- pixel art,
- abstract scientific visualisation,
- stylised map rendering,
- a hybrid of symbols and small animal sprites,
- another visual language discovered during prototyping.

The roadmap should not depend on choosing one yet.

## Rendering requirements regardless of art style

Whatever presentation is chosen, it must support the following.

### Ecosystem readability

At a glance, the player should be able to understand broad patterns such as:

- where animals are concentrated,
- where food and water are abundant,
- where populations are moving,
- which regions are harsh or thriving,
- how seasons and climate are changing the world.

### Individual readability

When zoomed in or following an animal, the player must be able to identify and inspect individuals.

### Large populations

The presentation must remain legible when thousands of entities are active.

### Deep-time observation

High-speed simulation should not require every simulated movement to be rendered literally. The renderer may aggregate, interpolate, or reduce visual detail as simulation speed increases.

### Seasonal and environmental feedback

Meaningful environmental changes should be visually apparent even if the final style remains abstract.

### Performance isolation

Rendering must not change simulation outcomes or become required for simulation progress.

## Fixed tick vs visual frame rate

The simulation should advance using a deterministic fixed tick.

The renderer may run at an independent frame rate and interpolate visual positions between simulation states.

Example:

- simulation: entity moves from cell `(10, 4)` to `(11, 4)` on a tick,
- presentation: visually interpolate that movement over several frames.

The interpolation is cosmetic and must never feed back into ecological simulation state.

## Zoom concept

A future prototype should test multiple information scales.

### Far zoom

Prioritise ecosystem patterns, population movement, biome boundaries, and density.

### Medium zoom

Show individual animal symbols clearly enough to follow groups and movement.

### Close zoom

Allow one animal to become the focus of inspection, with richer state and lineage information.

The exact graphical treatment at each zoom is deliberately unresolved.

## UI philosophy

UI should function like an observer's scientific toolkit rather than a management dashboard.

Useful surfaces may include:

- selected-animal panel,
- species/population panel,
- lineage viewer,
- graphs,
- timeline,
- map overlays,
- simulation-speed controls,
- seed and world information.

The UI should explain what nature is doing, not present a list of problems for the player to fix.
