# Vivarium Alpha Roadmap

> **Purpose:** Turn the LivingSim-Eco simulation into an observer game that is enjoyable to watch, investigate, understand, and become attached to.

Vivarium Alpha is **not Slice 12**. It is the next production roadmap after the LivingSim-Eco simulation foundation has passed its Foundation Exit Gate and has been frozen as `LivingSim-Eco v0.1`.

The foundation answers:

> **Can the ecosystem work autonomously?**

Vivarium Alpha answers:

> **Can someone enjoy watching and understanding that ecosystem for hours without controlling it?**

---

## 1. Entry Point — Simulation Foundation

Before Alpha production begins:

**Slices 0–11**  
→ Simulation Foundation

**Foundation Exit Gate**  
→ prove the ecosystem works across the required ecological, deterministic, performance, recovery, climate, evolution, and cross-seed gates

**`LivingSim-Eco v0.1` Foundation Freeze**  
→ lock the simulation baseline, benchmark seeds, hashes, save format, performance targets, and regression tests

Only after this handoff does Vivarium Alpha become active production work.

### Literal next action

When Slice 11 is complete:

> **Run and document the LivingSim-Eco Foundation Completion Test across the benchmark seed cohort.**

If it passes, freeze/tag `LivingSim-Eco v0.1` and begin **Alpha Slice 0 — Foundation Validation**.

If it fails, return to Foundation Gate Recovery. Do not begin Alpha production.

---

# 2. Alpha Slice 0 — Foundation Validation

Formalise the handoff from the simulation project into the observer-game project.

### Goals

- verify the final benchmark cohort
- archive seed histories and state hashes
- confirm save/reload continuation
- record performance baselines
- confirm the simulation baseline is reproducible
- establish the Alpha regression suite
- establish the first production build configuration

### Exit condition

The simulation foundation is treated as a protected production dependency rather than an endlessly moving target.

**Detailed production specification:** [Foundation Freeze & Handoff](01-foundation-freeze-and-handoff.md)

---

# 3. Alpha Slice 1 — Vivarium Shell

Build the actual application experience around the simulation.

### Core experience

- seed entry
- create world
- load world
- simulation screen
- pause/resume
- simulation-speed controls
- extreme fast-forward
- save/load session flow
- safe handling of invalid or corrupted saves
- clean transition between menus and the simulation

### Important constraint

**No final visual style is required yet.**

The goal is to prove the application loop, not solve the art direction.

The player should be able to:

`Enter seed → create world → watch → pause → accelerate → save → reload → continue`

### Exit condition

A fresh player can start and resume a seeded world without developer knowledge, while simulation determinism remains intact.

**Detailed production specification:** [Phase 1 — Application Shell](02-phase-1-application-shell.md)

---

# 4. Alpha Slice 2 — World Navigation

Make the world comfortable to explore.

### Navigation

- camera movement
- zoom in/out
- sensible camera limits
- click/tap selection
- entity highlighting
- follow-animal mode
- jump to population
- jump to important location
- return to world overview

### Observation overlays

- biome
- resources
- population density
- climate
- selected species
- relevant ecological states

### Design goal

This is where the experience should begin to feel like an **aquarium / living-world viewer**.

The player isn't managing the world.

They are choosing **where to look**.

### Exit condition

A player can comfortably move from:

**world → region → population → individual**

and back again without losing their sense of place.

**Detailed production specification:** [Phase 2 — Observation](03-phase-2-observation.md)

---

# 5. Alpha Slice 3 — Animal Observation

Make individual animals worth following.

Selecting an animal should expose a readable identity card/profile such as:

## Wolf #7182

- age
- sex
- generation
- health
- energy
- current behaviour
- species
- important traits
- parents
- offspring
- population
- current region
- relevant life events

### Follow mode

The player should be able to select:

**Follow Wolf #7182**

and simply watch its life unfold.

The system should make it possible to understand:

- where it travels
- what it eats
- what threatens it
- when it reproduces
- who its parents were
- whether it produces offspring
- how long it survives
- how its life ends

### Design goal

Thousands of simulated animals become meaningful through individual stories.

### Exit condition

A player can select an animal and follow a comprehensible life story from birth through death.

**Detailed production specification:** [Phase 2 — Observation](03-phase-2-observation.md)

---

# 6. Alpha Slice 4 — Population & Evolution Tools

Expose the deeper simulation without forcing the player to read raw debug data.

### Population tools

- population graphs
- species population history
- predator/prey graphs
- biomass graphs
- population density
- geographic population ranges
- birth/death rates
- extinction events

### Evolution tools

- trait averages
- trait distributions
- generation tracking
- trait change through time
- regional trait comparison
- population comparison

The player should be able to discover things like:

> **Wolf speed increased sharply after Year 870.**

and then investigate why.

### Exit condition

A player can move from an observed change to the relevant population/trait data and understand the basic trend.

**Detailed production specification:** [Phase 2 — Observation](03-phase-2-observation.md)

---

# 7. Alpha Slice 5 — Lineages

This is one of the core features of the Vivarium concept.

### Lineage information

Allow the player to inspect:

- parents
- children
- ancestry
- descendants
- generations
- founder populations
- lineage survival
- lineage extinction

### Example experience

Select a wolf in Year 4.

Fast-forward to Year 1,200.

Discover:

> **18% of the living wolves descend from this lineage.**

The player should be able to move through the family history rather than seeing ancestry as a static number.

### Design goal

Turn genetic simulation into **personal history**.

The ecosystem becomes a world containing stories rather than anonymous statistics.

### Exit condition

A player can trace an individual or lineage across generations and understand its survival, expansion, decline, or extinction.

**Detailed production specification:** [Phase 3 — History & Lineage](04-phase-3-history-and-lineage.md)

---

# 8. Alpha Slice 6 — Natural History

Turn raw simulation events into readable ecological stories.

The game should automatically recognise important events and present them as history.

### Example

> **Year 184 — Severe drought begins**  
> **Year 203 — Rabbit population falls 61%**  
> **Year 219 — Western wolf population reaches historic low**  
> **Year 287 — Rabbit recovery begins**  
> **Year 431 — Northern deer lineage diverges significantly**  
> **Year 902 — Eastern wolves become extinct**

### Systems

- event detection
- event significance scoring
- natural-history timeline
- population milestones
- extinction records
- recovery records
- migration records
- evolutionary milestones
- major environmental events

### Design goal

The simulation should effectively be capable of **writing a nature documentary about itself**.

### Exit condition

A player can look back through a world and understand its major ecological history without manually inspecting every graph.

**Detailed production specification:** [Phase 3 — History & Lineage](04-phase-3-history-and-lineage.md)

---

# 9. Alpha Slice 7 — Visual Prototyping

**Do not lock the visual identity before this point.**

Use the exact same simulation/world and test multiple renderers against it.

The first visual question is not:

> “What looks coolest?”

It is:

> **“What lets thousands of living things remain readable while preserving the character of the simulation?”**

## Prototype A — Graphical ASCII / Glyphs

A custom ASCII-inspired renderer outside the terminal.

Characteristics:

- simple glyphs
- strong silhouettes
- limited palette
- readable terrain symbols
- species-specific animal glyphs
- overlays
- scalable map
- Dwarf Fortress-inspired information density without copying its presentation

## Prototype B — Minimal Pixel Ecosystem

Small, readable pixel-art animals and terrain.

Characteristics:

- simple sprites
- strong silhouettes
- restrained animation
- high information density
- easy zooming

## Prototype C — Scientific Map

A more abstract ecological visualization.

Characteristics:

- terrain as map information
- animals as clear markers
- data-forward overlays
- population/ecology visualisation
- deliberately less characterful

## Prototype D — Hybrid

Combine levels of abstraction.

For example:

**far away:** glyph/map representation

**medium zoom:** readable animal sprites

**close zoom:** richer individual presentation

### Visual evaluation

Each prototype should be tested using the same:

- world
- seed
- population
- simulation state
- camera positions
- observation tasks

Compare:

- readability
- information density
- emotional attachment
- species recognition
- terrain recognition
- performance
- zoom behaviour
- visual clutter
- long-session fatigue

### Exit condition

Select one direction using evidence rather than preference alone.

**Detailed production specification:** [Phase 4 — Visual Prototyping](05-phase-4-visual-prototyping.md)

---

# 10. Alpha Slice 8 — Presentation Lock

Once the visual direction wins, build the actual renderer and presentation layer.

### Rendering

- production renderer
- scalable zoom
- readable massive populations
- smooth movement/interpolation
- consistent glyph/sprite scale
- visual hierarchy
- selection states
- follow states
- overlays

### Environment presentation

- seasons
- weather/climate states
- vegetation changes
- resource changes
- water states
- day/night presentation where useful

### UI

- polished inspection panels
- readable graphs
- history interface
- lineage interface
- timeline
- world overview
- unobtrusive information hierarchy

### Exit condition

The project should now visibly resemble the actual game rather than a development prototype.

**Detailed production specification:** [Phase 5 — Watchability & Polish](06-phase-5-watchability-and-polish.md)

---

# 11. Alpha Phase 5 — Make It Enjoyable to Watch

Presentation is not finished when the renderer works.

The actual goal is **watchability**.

### Time

- smooth normal-speed simulation
- understandable fast-forward
- clear transition between speeds
- readable events during accelerated time
- ability to stop time instantly
- no loss of important history when skipping time

### Environmental feedback

The player should visually understand:

- seasonal change
- population change
- resource depletion
- resource recovery
- climate pressure
- migration
- ecological collapse
- recovery

### Attachment

The experience should encourage players to care about:

- an individual animal
- a family
- a lineage
- a species
- a region
- an ecosystem

### Fresh-eye testing

Give new players the game without explaining the intended experience.

Observe whether they naturally:

- follow animals
- inspect populations
- compare species
- use graphs
- investigate events
- trace lineages
- fast-forward
- return to previous locations
- tell stories about what happened

### Exit condition

Players voluntarily continue observing because the simulation creates questions they want answered.

**Detailed production specification:** [Phase 5 — Watchability & Polish](06-phase-5-watchability-and-polish.md)

---

# 12. Alpha Exit Gate

Vivarium Alpha is complete when the observer experience is proven.

The player must be able to:

- create a seeded world
- load a world
- navigate comfortably
- understand the basic environment
- select an animal
- follow an animal
- inspect populations
- inspect traits
- investigate ecological change
- trace ancestry/descendants
- review natural-history events
- use graphs and overlays
- fast-forward through deep time
- save and reload
- understand important changes without developer assistance

And most importantly:

> **The player should want to keep watching.**

Alpha should prove that the game is compelling **without intervention mechanics**.

**Detailed proof:** [Alpha Exit Gate](11-alpha-exit-gate.md)

---

# 13. Only After Alpha — Ecological Breadth

Do **not** immediately expand the simulation after the foundation passes.

First prove the observer experience.

Once Alpha is successful, unlock broader ecological development.

Potential expansion:

- more species
- specialised predators
- scavengers
- fish
- aquatic food webs
- insects
- disease
- wildfire
- deeper migration
- richer plant ecology
- more climates
- stronger biome niches
- potentially speciation
- more sophisticated population behaviour

Every new system should deepen the existing ecosystem rather than simply increase the content count.

**Entry gate:** [Ecological Breadth Entry Gate](07-ecological-breadth-entry-gate.md)

---

# 14. Full Product Progression

The intended development path is:

```
Slices 0–11
    ↓
Simulation Foundation
    ↓
Foundation Exit Gate
    ↓
Foundation Recovery if required
    ↓
LivingSim-Eco v0.1
    ↓
Foundation Freeze
    ↓
Vivarium Alpha Slice 0
    ↓
Alpha Slice 1 — Vivarium Shell
    ↓
Alpha Slice 2 — World Navigation
    ↓
Alpha Slice 3 — Animal Observation
    ↓
Alpha Slice 4 — Population & Evolution Tools
    ↓
Alpha Slice 5 — Lineages
    ↓
Alpha Slice 6 — Natural History
    ↓
Alpha Slice 7 — Visual Prototyping
    ↓
Alpha Slice 8 — Presentation Lock
    ↓
Alpha Watchability / Polish
    ↓
Alpha Exit Gate
    ↓
Visual Direction Locked
    ↓
Ecological Breadth
    ↓
Beta
    ↓
1.0
```

---

# 15. Production Rules

Vivarium Alpha uses professional large-studio techniques where they improve quality and predictability, but avoids AAA-scale bureaucracy.

### Use

- vertical slices
- milestone entry/exit gates
- definition of ready
- definition of done
- performance budgets
- regression baselines
- automated smoke tests
- long-run soak tests
- usability testing
- profiling builds
- feature flags where useful
- bug severity and triage
- content locks
- risk registers
- save-version discipline
- accessibility from the start
- repeatable benchmark scenarios
- fresh-eye testing
- evidence-based visual decisions

### Do not

- add systems because they sound cool
- lock art direction before testing alternatives
- let UI work modify simulation outcomes
- expand species count to hide ecological problems
- build final content before the observer loop works
- create AAA-style process overhead for its own sake

> **AAA-quality discipline, not AAA-sized bureaucracy.**

---

# 16. The Central Rule

Once the simulation foundation passes, **stop asking “what more simulation can we add?”**

Ask:

> **“How can we make the existing living world more observable, understandable, beautiful, surprising, and emotionally meaningful?”**

The simulation is the engine.

**Vivarium is the experience built around that engine.**

---

## Detailed Alpha Documents

- [00 - Alpha Charter](00-alpha-charter.md)
- [01 - Foundation Freeze & Handoff](01-foundation-freeze-and-handoff.md)
- [02 - Phase 1: Application Shell](02-phase-1-application-shell.md)
- [03 - Phase 2: Observation](03-phase-2-observation.md)
- [04 - Phase 3: History & Lineage](04-phase-3-history-and-lineage.md)
- [05 - Phase 4: Visual Prototyping](05-phase-4-visual-prototyping.md)
- [06 - Phase 5: Watchability & Polish](06-phase-5-watchability-and-polish.md)
- [07 - Ecological Breadth Entry Gate](07-ecological-breadth-entry-gate.md)
- [08 - Production Pipeline & Milestone Discipline](08-production-pipeline-and-milestones.md)
- [09 - Technical Quality Budgets](09-technical-quality-budgets.md)
- [10 - QA, UX & Accessibility](10-qa-ux-and-accessibility.md)
- [11 - Alpha Exit Gate](11-alpha-exit-gate.md)
- [12 - Risk Register & Scope Control](12-risk-register-and-scope-control.md)
