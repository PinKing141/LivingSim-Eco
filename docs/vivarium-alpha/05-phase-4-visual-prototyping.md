# 05 - Phase 4: Visual Prototyping & Direction Lock

## Goal

Choose the final visual language through controlled prototypes instead of preference alone.

The same simulation state should be rendered through multiple visual approaches so the decision is based on readability, performance, atmosphere and watchability.

---

## Candidate directions

At minimum prototype:

1. **Graphical custom ASCII/glyphs**
   - Dwarf Fortress-inspired readability,
   - custom bitmap/atlas glyphs,
   - colour-coded terrain and animals,
   - high density and strong world-scale readability.

2. **Minimalist pixel art**
   - small terrain and animal sprites,
   - limited animation,
   - stronger individual character readability.

3. **Scientific-map style**
   - abstract symbols,
   - restrained visual noise,
   - strong overlays/data integration.

4. **Hybrid zoom system**
   - abstract/glyph-based world view,
   - richer close-range representation,
   - seamless transition between ecological and individual scales.

A fifth prototype may be added only if it tests a genuinely different visual thesis.

---

## Prototype rules

All prototypes must use:

- the same benchmark seed,
- the same camera framing,
- the same animal positions,
- the same time of year,
- the same information overlays,
- the same performance capture method.

Do not compare one polished concept against one rough concept.

Create equivalent prototype maturity.

---

## Evaluation criteria

Score each direction against weighted criteria.

### Readability

- Can biomes be distinguished immediately?
- Can animals be found in dense terrain?
- Can predator/prey classes be scanned quickly?
- Does selection remain obvious?
- Can the player read the world while time is moving quickly?

### Scale

- Does the style work with hundreds/thousands of visible entities?
- Is the whole-world view still meaningful?
- Can it support future species without visual collapse?

### Watchability

- Is it pleasant to stare at for long sessions?
- Does motion feel alive rather than noisy?
- Does the style make ecological change noticeable?

### Identity

- Does it feel recognisably LivingSim rather than a generic strategy map?
- Is the style coherent with the observer-naturalist fantasy?

### Performance

- render cost,
- batching/draw-call behaviour,
- memory footprint,
- zoom cost,
- animation cost,
- overlay compositing cost.

### Production cost

- asset creation time,
- animation burden,
- consistency burden,
- difficulty adding new species/biomes,
- accessibility variants.

---

## Custom ASCII/glyph prototype requirements

Because this is currently a strong candidate, prototype it seriously rather than as terminal text.

Test:

- custom glyph atlas rather than OS font dependency,
- fixed logical grid with graphical rendering,
- foreground/background colour separation,
- terrain layers,
- entity layer,
- selection/focus overlay,
- smooth visual interpolation between simulation cells where desired,
- multiple zoom levels,
- readable custom biome symbols,
- readable custom animal symbols,
- colour-independent identity where possible.

The renderer should be able to preserve an ASCII-like visual language without being limited by an actual terminal.

---

## Readability lab scenes

Create fixed test scenes:

- dense forest with many animals,
- coast/river/wetland transition,
- snowy mountain region,
- large herbivore herd,
- predator chase,
- carcass-heavy mortality event,
- multiple overlays,
- extreme zoom-out,
- individual follow close-up.

Every visual prototype must be tested on every scene.

---

## Accessibility evaluation

Test each style for:

- deuteranopia/protanopia/tritanopia simulation,
- grayscale readability,
- low contrast conditions,
- UI scale changes,
- reduced motion,
- small-screen/windowed readability if supported.

Never make species or biome identity depend on hue alone.

---

## Performance capture

For each prototype capture:

- FPS/frame time at target visible population,
- CPU render/update time,
- GPU frame time where available,
- memory footprint,
- extreme-fast-forward presentation overhead,
- worst-case overlay cost.

The prettiest solution is not acceptable if it materially compromises the simulation scale.

---

## Direction-lock process

Use a written comparison table and record:

- strengths,
- weaknesses,
- measured performance,
- tester preference,
- readability results,
- future content cost,
- accessibility risks.

Then choose one direction and explicitly record why.

After the direction lock:

- stop running parallel art styles,
- create a visual language guide,
- define glyph/sprite sizing,
- define palette rules,
- define UI/map contrast rules,
- define animation rules,
- create an asset naming/atlas convention.

---

## Phase exit gate

Phase 4 passes when:

- at least four comparable visual prototypes exist,
- each has been tested using the same benchmark scenes,
- performance and readability data are recorded,
- accessibility checks are complete,
- one visual direction is formally selected,
- a lightweight visual style guide exists,
- the choice supports both ecosystem-scale and individual-scale observation.

Only then begin Phase 5.