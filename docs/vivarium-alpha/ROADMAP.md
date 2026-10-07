# Vivarium Alpha — Master Roadmap

## Purpose

Vivarium Alpha is the phase after the LivingSim-Eco simulation foundation has passed every Foundation Exit Gate and been frozen as `LivingSim-Eco v0.1`.

The foundation proves:

> **Can the ecosystem work autonomously, deterministically, at scale, and across deep time?**

Vivarium Alpha asks a different question:

> **Can someone enjoy watching, investigating, understanding, and becoming attached to that living world for hours without controlling it?**

This is **not Slice 12**. It is a new production phase with its own milestones, quality bar, and exit gate.

---

# 0. Foundation Validation

Before Alpha production begins:

- Run the complete Foundation Completion/Exit Test.
- Use the locked benchmark seed cohort.
- Confirm deterministic replay.
- Confirm save/reload continuation.
- Confirm performance baselines.
- Confirm ecological viability, recovery, evolution, climate response, geographic divergence, and cross-seed diversity.
- Archive the results.
- Tag the passing simulation as `LivingSim-Eco v0.1`.
- Freeze the simulation rules as the Alpha baseline.

**Important:** Visual experiments and design exploration may happen before this point, but production Alpha work does not begin until the foundation passes.

---

# 1. Alpha Slice 1 — Vivarium Shell

Turn the simulation into an actual application experience.

### Core flow

- Seed entry
- Create world
- Generate world
- Enter simulation
- Load existing world
- Basic save/session flow
- Return to menu
- Safe exit

### Simulation controls

- Pause
- Resume
- Normal speed
- Fast-forward
- Extreme fast-forward
- Clear current simulation speed
- Time progression feedback

### Goal

The user should be able to:

> **Enter a seed → create a world → start the simulation → pause/accelerate it → save it → return → load it.**

No final visual style is required yet.

### Exit condition

The complete application loop works reliably without needing developer tools or terminal interaction.

---

# 2. Alpha Slice 2 — World Navigation

Make the world comfortable to explore.

### Camera

- Pan
- Zoom in/out
- Stable camera movement
- Sensible zoom limits
- Smooth or deliberately stepped movement depending on the final visual style

### World interaction

- Click/select entities
- Hover information where useful
- Jump to a population
- Jump to an animal
- Follow an animal
- Return to world view

### Observation overlays

- Biomes
- Biomass/resources
- Water
- Population density
- Predator/prey distribution
- Climate/environment state

### Goal

The world should start feeling like an **aquarium rather than a debug visualizer**.

### Exit condition

A user can comfortably spend time simply moving around and watching the ecosystem.

---

# 3. Alpha Slice 3 — Animal Observation

Make individual animals interesting.

Selecting an animal should provide a clear observation panel.

Example:

**Wolf #7182**

- Age
- Sex
- Generation
- Health
- Energy
- Current state/behaviour
- Traits
- Parents
- Offspring
- Population/species
- Relevant lineage information

### Follow mode

Allow the user to:

- select an animal
- follow it
- watch it move
- watch it hunt
- watch it feed
- watch it reproduce
- watch it age
- watch it die

The goal is to make:

> **“I wonder what happens to this animal?”**

a natural player reaction.

### Exit condition

A selected animal can be followed through a meaningful portion of its life without the interface becoming confusing or unusable.

---

# 4. Alpha Slice 4 — Population & Evolution Tools

Expose the interesting information already being produced by the simulation.

### Population

- Population over time
- Species population
- Regional population
- Birth/death rates
- Predator/prey relationships

### Environment

- Biomass
- Resource levels
- Climate
- Temperature/seasonal trends where supported
- Geographic population ranges

### Evolution

- Trait averages
- Trait distributions
- Trait change over generations
- Regional trait differences
- Species-level evolutionary trends

Example discovery:

> **Wolf speed increased sharply after Year 870.**

The player should be able to investigate why.

### Exit condition

A user can answer questions such as:

- Why did this population grow?
- Why did it collapse?
- Which traits changed?
- Where is this species concentrated?
- What happened before the change?

without opening developer/debug tools.

---

# 5. Alpha Slice 5 — Lineages

Turn individual simulation data into long-term stories.

### Family relationships

- Parents
- Children
- Siblings where supported
- Ancestors
- Descendants

### Lineage tools

- Founder populations
- Generation tracking
- Lineage survival
- Lineage extinction
- Descendant counts
- Historical lineage searches

Example:

> Select a wolf in Year 4, fast-forward to Year 1,200, and discover that 18% of the living wolf population descends from that lineage.

### Goal

Make evolutionary history personally meaningful.

The player should begin caring about particular individuals and lineages even though they never directly control them.

### Exit condition

A user can trace an individual or lineage through substantial periods of simulation history.

---

# 6. Alpha Slice 6 — Natural History

Turn raw simulation data into readable stories.

The system should recognise meaningful events rather than requiring the player to interpret every graph manually.

### Event examples

> Year 184 — Severe drought begins

> Year 203 — Rabbit population falls 61%

> Year 219 — Western wolf population reaches historic low

> Year 287 — Rabbit recovery begins

> Year 431 — Northern deer lineage shows significant divergence

> Year 902 — Eastern wolves become extinct

### Event categories

- Population boom
- Population crash
- Recovery
- Local extinction
- Global extinction
- Migration
- Climate event
- Resource collapse
- Resource recovery
- Major evolutionary shift
- Lineage milestone

### Timeline

Provide a readable history of the world.

The simulation should increasingly feel like it is creating its own nature documentary.

### Exit condition

A player can look back over centuries and understand the major events that shaped the world.

---

# 7. Alpha Slice 7 — Visual Prototyping

**Only now should the visual identity be seriously selected.**

Use the **same simulation and the same benchmark worlds** for every prototype.

The simulation must not change to make a visual concept look better.

## Prototype A — Graphical ASCII / Glyphs

A custom ASCII-inspired renderer outside the terminal.

Characteristics to test:

- Strong silhouettes
- Monospace presentation
- Limited colour palette
- Clear biome glyphs
- Distinct animal glyphs
- Dense information at zoomed-out scale
- Dwarf Fortress-inspired readability without copying its exact presentation

## Prototype B — Minimal Pixel Ecosystem

Small, readable pixel-art animals and terrain.

Priorities:

- silhouette readability
- low visual noise
- clear species identity
- scalable zoom
- large populations remaining readable

## Prototype C — Scientific Map

A more abstract ecological visualisation.

Priorities:

- geography
- population density
- biome boundaries
- ecological overlays
- data readability

## Prototype D — Hybrid

Use different representation levels at different zoom levels.

For example:

- distant → ecological symbols/glyphs
- medium → simple animal representations
- close → more detailed animal presentation

### Comparison

Evaluate each prototype using the same criteria:

- readability
- world-scale clarity
- animal identification
- population readability
- performance
- visual personality
- ability to communicate ecological change
- ability to support long observation sessions

### Exit condition

Select one visual direction using evidence from playable prototypes rather than preference alone.

---

# 8. Alpha Slice 8 — Presentation Lock

Once the visual direction wins, build the real presentation layer.

### Rendering

- Proper renderer
- Scalable world
- Smooth movement where appropriate
- Stable camera
- Clear entity silhouettes
- Population readability
- Visual hierarchy

### Environment

- Seasons
- Climate changes
- Resource changes
- Environmental transitions
- Major ecological events

### Interface

- Polished inspection UI
- Clear graphs
- Clear overlays
- Readable timelines
- Good navigation
- Consistent interaction language

### Goal

LivingSim should stop looking like a simulation prototype and start looking like the actual game.

### Exit condition

The visual language is consistent, readable, performant, and suitable for the intended scale of the ecosystem.

---

# After Alpha — Ecological Breadth

**Do not expand the ecosystem substantially until Vivarium Alpha proves that watching the existing ecosystem is fun.**

Once the Alpha Exit Gate passes, begin the broader content phase.

Potential additions include:

- More species
- Specialised predators
- Scavengers
- Fish
- Aquatic food webs
- Insects
- Disease
- Wildfire
- Deeper migration
- Richer plant ecology
- More climates
- More biome niches
- Potential speciation
- More sophisticated population behaviour

The rule is:

> **Every new ecological system must create meaningful interactions with the existing ecosystem.**

Do not add content simply to increase the species count.

---

# Beta

After ecological breadth has been proven:

- Stabilise systems
- Harden saves
- Improve accessibility
- Improve onboarding
- Improve settings
- Improve performance across target hardware
- Expand QA coverage
- Polish UI
- Polish visual presentation
- Improve error handling
- Test long observation sessions
- Test fresh-player comprehension
- Prepare release builds
- Lock major gameplay systems

Beta should primarily be about **quality, stability, usability and polish**, not discovering the game's core identity.

---

# 1.0

The intended progression is:

```
LivingSim-Eco
    ↓
11 Simulation Slices
    ↓
Foundation Recovery / Validation
    ↓
Foundation Exit Gate
    ↓
LivingSim-Eco v0.1
    ↓
Vivarium Alpha
    ↓
Visual Direction Lock
    ↓
Ecological Breadth
    ↓
Beta
    ↓
1.0
```

---

# The Core Product Question

The foundation asks:

> **Can nature work?**

Vivarium Alpha asks:

> **Can watching nature be a game?**

The final product should answer:

> **Can a player become fascinated by a world they cannot control?**

That is the core identity of LivingSim.

---

# Literal Next Action

When Slice 11 and all Foundation Recovery work are complete:

**Do not immediately start Alpha development.**

First:

1. Run the Foundation Completion/Exit Test.
2. Document the complete results.
3. Verify every Foundation Gate.
4. If it passes, tag `LivingSim-Eco v0.1`.
5. Freeze the simulation baseline.
6. Preserve the benchmark seeds and hashes.
7. Begin **Vivarium Alpha Slice 1 — Vivarium Shell**.

If the Foundation Exit Gate fails, return to the specific failing Foundation Gate instead.

**Never use Alpha development to hide an unresolved simulation-foundation failure.**
