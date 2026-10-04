# 07 - Development Roadmap

The roadmap is ordered so every slice produces a testable simulation improvement while avoiding dependency on later presentation decisions.

Each slice should be completed and stabilised before systems that depend on it are added.

---

## Slice 0 - Preserve the prototype

**Status: Complete (2026-10-04).** The final pre-cleanup console prototype is preserved at Git tag `legacy-ascii-prototype`; its retained behaviours and intentional rewrite exclusions are documented in [00 - Legacy Prototype Baseline](00-legacy-prototype-baseline.md).

### Goal

Protect the old playable ASCII ecosystem as a reference while creating a clean foundation for the rewrite.

### Work

- Create a permanent legacy branch or tag from the last full pre-cleanup implementation.
- Keep the rewrite line separate rather than rebuilding new architecture inside the legacy code.
- Document useful behaviours from the prototype.
- Document prototype behaviours deliberately excluded from the rewrite.
- Keep legacy code available for behavioural comparison, not as an architectural template.

### Exit criteria

- The previous simulator is safely recoverable.
- The rewrite has a clean baseline.
- There is no ambiguity about which codebase is authoritative going forward.

---

## Slice 1 - Deterministic world foundation

**Status: Complete (2026-10-04).** The headless Slice 1 core uses versioned seed settings, compact world-cell storage, deterministic terrain/biome generation, fixed-point plant biomass regeneration, and an explicit immutable system order. Automated checks cover same-seed generation, 10,000-tick resource determinism, biomass bounds, system order, and absence of console use in the core.

### Goal

Create a world that can generate and advance deterministically before animals exist.

### Work

- Define seed handling.
- Create the fixed simulation clock.
- Define deterministic system execution order.
- Implement compact world-cell storage.
- Add initial terrain/biome generation.
- Add numeric plant biomass.
- Add resource regeneration.
- Separate simulation code completely from rendering.
- Add headless execution entry point or test harness.

### Tests

- Same seed generates identical world data.
- Same generated world produces identical resource state after N ticks.
- No simulation result depends on console or graphical output.

### Exit criteria

A world can generate and advance thousands of ticks with no animals and reproduce exactly from the same version and seed.

---

## Slice 2 - Two-species survival loop

**Status: Complete (2026-10-04).** The headless core now has a dense animal store, local spatial buckets, a herbivore and predator, movement, ageing, energy, health, local targeting, plant feeding, hunting, starvation, death, carcasses, and concise population debug output. Tests cover world bounds, plant consumption, valid prey kills, starvation, carcass consumption/decay, and deterministic full state after 1,000 ticks.

### Goal

Prove the minimum autonomous ecosystem with one herbivore and one predator.

### Work

- Add entity storage.
- Add herbivore species.
- Add predator species.
- Add position and movement.
- Add lifecycle and age.
- Add metabolism / energy.
- Add health.
- Implement local perception.
- Implement spatial partitioning.
- Add herbivore feeding from biomass.
- Add predator pursuit and hunting.
- Add death.
- Add carcasses.
- Add only the minimum debug output needed to inspect the simulation.

### Scope limits

Do **not** add yet:

- complex genetics,
- packs,
- territories,
- memory,
- scents,
- advanced UI,
- multiple extra species.

### Tests

- Animals cannot leave world bounds.
- Herbivores consume biomass.
- Predators can acquire and kill valid prey.
- Starvation causes death.
- Carcasses appear and decay/are consumed according to rules.
- Same seed produces the same state after N ticks.

### Exit criteria

Predator and prey populations can live and die without scripted outcomes.

---

## Slice 3 - Reproduction & inheritance

### Goal

Turn the survival simulation into a multi-generation system.

### Work

- Add lifecycle maturity.
- Define reproduction eligibility.
- Add mate selection or the intentionally simplified first model.
- Add reproduction energy cost.
- Add offspring creation.
- Add parent links if affordable at this stage.
- Introduce a small set of inheritable traits.
- Add bounded mutation.
- Track generation count.
- Track population trait averages.

### First trait set

Keep the initial set small, for example:

- speed,
- metabolism,
- vision,
- size,
- fertility.

Every trait must have an ecological trade-off.

### Tests

- juveniles cannot reproduce,
- inheritance stays inside hard bounds,
- mutation is deterministic,
- reproduction applies the intended cost,
- multi-generation simulations remain deterministic.

### Exit criteria

Populations survive for multiple generations and trait averages visibly drift over long runs.

---

## Slice 4 - Ecological stability

### Goal

Make the small ecosystem capable of producing understandable long-term behaviour rather than immediate extinction or infinite growth.

### Work

- Balance plant biomass regeneration.
- Balance metabolism.
- Balance predation success.
- Balance reproduction rates.
- Add local overgrazing / habitat pressure.
- Add recovery from depleted habitat.
- Measure population oscillation.
- Build long-run simulation reporting.

### Long-run checks

Detect cases such as:

- unbounded population growth,
- all animals starving immediately,
- predators always exterminating prey,
- predators never being able to survive,
- biomass permanently stuck at zero,
- populations frozen into static equilibrium.

The game does not need every seed to remain populated forever. Extinction is valid. The target is **interesting ecological variation**, not guaranteed stability.

### Exit criteria

Multiple seeds produce varied but interpretable histories including booms, crashes, recovery, or extinction rather than one universal failure mode.

---

## Slice 5 - Broader food web

### Goal

Expand the ecosystem only after the two-species model is proven.

### Work

- Add additional herbivore niches.
- Add additional predator niches.
- Add omnivory where useful.
- Add scavenging where useful.
- Define species through trait ranges and ecological parameters rather than unique bespoke AI whenever possible.
- Validate competition between species.
- Validate niche differences.

### Design rule

A new species should change the ecological system.

Do not add species purely for visual variety.

Examples of meaningful differences:

- high-speed / high-metabolism predator,
- slow large herbivore,
- small fast-breeding herbivore,
- generalist omnivore,
- specialised scavenger.

### Exit criteria

A multi-species food web produces competition, predation, scavenging, and local ecological niches.

---

## Slice 6 - Social & population behaviour

### Goal

Introduce lightweight collective behaviour where it materially improves the ecosystem.

### Work

Potential systems include:

- herd/flock cohesion,
- pack cohesion,
- separation / collision avoidance,
- local group identity,
- population/range identity,
- migration when local conditions deteriorate.

### Scope guard

Avoid adding complex social simulation just because the old prototype had it.

Do not initially require:

- leadership politics,
- alpha hierarchies,
- detailed relationship memory,
- teaching,
- social reputation,
- elaborate territory politics.

Those systems should only return if observation demonstrates a real ecological need.

### Exit criteria

Populations move and organise in ways that are visually and ecologically distinct without compromising scale.

---

## Slice 7 - Climate & deep time

### Goal

Make the environment change strongly enough over long periods to create new selection pressures.

### Work

- Add seasons if not already active.
- Add season-specific biomass modifiers.
- Add slow climate oscillation.
- Add drought and abundance periods.
- Add temporary habitat degradation.
- Add habitat recovery.
- Record climate history.
- Measure trait response to environmental pressure.

### Questions to validate

- Do droughts actually favour lower-metabolism animals?
- Do repeated harsh periods alter average body size or fertility?
- Do predators evolve differently when prey become faster?
- Can geographically separate populations respond differently to the same climate era?

### Exit criteria

Hundreds of simulated years create recognisable environmental eras and measurable evolutionary responses.

---

## Slice 8 - Observer interface

### Goal

Turn the simulation into something a player can meaningfully watch and investigate without changing it.

### Work

- Pause/resume.
- Simulation speed controls.
- Extreme fast-forward.
- Camera/navigation.
- Animal selection.
- Follow-animal mode.
- Species/population inspection.
- Basic trait graphs.
- Population graphs.
- Resource overlays.
- Population-density overlays.
- Climate overlays where useful.

### Presentation constraint

This slice does not require the final art style.

A functional temporary renderer is acceptable as long as the simulation remains independent from it.

### Exit criteria

A player can understand what the ecosystem is doing without being able to control ecological outcomes.

---

## Slice 9 - History & lineage

### Goal

Allow the game to explain its own natural history.

### Work

- Persist scalable parent/offspring information.
- Add ancestry inspection.
- Add descendant tracing.
- Track population identity over time.
- Add notable-event detection.
- Add timeline UI/data.
- Record major population highs and lows.
- Record extinction.
- Record major migration.
- Record trait milestones.
- Allow timeline or graph events to focus relevant world locations/populations where practical.

### Storage rule

Do not store every trivial event forever.

Retain information according to its usefulness for later observation.

### Exit criteria

The player can investigate not only what exists now but how the world reached its current state.

---

## Slice 10 - Presentation shell

### Goal

Choose and implement the final viewing experience only after simulation and observer requirements are proven.

### Work

- Prototype multiple visual approaches.
- Choose the rendering stack based on actual requirements.
- Move out of terminal rendering if that remains the preferred direction.
- Implement scalable zoom levels.
- Add smooth visual interpolation.
- Improve seasonal/environmental feedback.
- Improve animal readability.
- Add polished inspection surfaces.
- Ensure extreme fast-forward remains visually understandable.

### Important

ASCII-inspired rendering is one candidate, not a locked requirement.

The final choice should be made through prototypes.

### Exit criteria

The simulation is pleasant to watch for long periods and readable at both ecosystem and individual scales.

---

## Slice 11 - Scale, save/load & release loop

### Goal

Turn the prototype into a stable long-running vivarium product.

### Work

- Profile thousands of entities.
- Remove remaining hot-path allocations.
- Optimise spatial queries.
- Add save/load.
- Store simulation-version metadata.
- Add deterministic continuation tests.
- Create benchmark seeds.
- Create automated soak tests.
- Test very long simulations.
- Polish seed-entry onboarding.
- Polish observation controls.
- Define the first-release species/content scope.
- Stop adding systems that are not necessary for the release loop.

### Release benchmark categories

Maintain a small set of known seeds for:

- performance,
- high population density,
- predator-prey stability,
- extinction scenarios,
- drought stress,
- evolutionary trait movement,
- save/load determinism.

### Exit criteria

A stable build can create, run, save, reload, and revisit long-lived vivariums at the target population scale.

---

# End goal of the 11-slice roadmap

Slices 0-11 are the **LivingSim-Eco simulation foundation and proof-of-concept milestone**. They are not the complete final game and should not continually expand to absorb every future idea.

The purpose of finishing this roadmap is to prove the central product question:

> **Can a seeded ecosystem be interesting enough to watch for long periods when the player has no ecological control over it?**

The roadmap is considered complete only when the following are true:

- A player can enter or generate a seed and create a deterministic world.
- The world can run for thousands of simulated years without requiring player intervention.
- Multiple species can survive, compete, reproduce, migrate, hunt, scavenge, die out, and recover according to systemic rules rather than scripted outcomes.
- Evolution is visible across generations through measurable trait changes caused by selection pressure.
- Climate, resources, geography, predation, reproduction, and population density interact strongly enough to create cascading ecological consequences.
- Different seeds can produce meaningfully different natural histories rather than converging on the same repeating pattern.
- Valid outcomes include coexistence, population booms, crashes, recovery, local extinction, total extinction, migration, and long-term evolutionary shifts.
- The player can investigate **why** something happened through population graphs, trait data, environmental history, lineages, and notable events.
- Individual animals can be inspected and followed without making the simulation dependent on individual-character micromanagement.
- The simulation remains deterministic and headless-capable.
- Long-run performance supports the intended population scale without the observer/presentation layer controlling simulation behaviour.
- A vivarium can be saved, reloaded, continued, and revisited without changing its deterministic future for the same simulation version.
- The experience is already compelling with a deliberately limited species/content roster.

## Completion test

Before starting another major roadmap, run a small evaluation set of different seeds for long periods.

The milestone passes if those runs naturally produce substantially different, explainable histories such as:

- one seed reaching long-term predator/prey coexistence,
- another losing a predator lineage,
- another experiencing a climate-driven population bottleneck,
- another showing a clear evolutionary response to sustained selection pressure,
- another producing migration or geographically separated populations with different trait trends.

If every seed mainly settles into the same repeating population cycle, the answer is **not** to add dozens of new species. The existing ecosystem needs more depth first.

## Scope discipline until completion

Until this milestone is reached, avoid turning the roadmap into a feature wishlist.

Not required to complete the 11-slice milestone:

- a huge species roster,
- the final art style,
- extensive cosmetic variety,
- player ecosystem intervention tools,
- achievements,
- extensive meta-progression,
- large amounts of handcrafted content,
- additional simulation layers that do not improve the core ecology.

New ideas can be recorded for later, but the priority remains completing and validating the existing slices.

## What comes after

Once the completion test passes, stop extending this roadmap and create a new roadmap for **Vivarium Alpha**.

Vivarium Alpha should focus on turning the proven simulation into the fuller product, including areas such as:

- final presentation and visual experimentation,
- stronger observer UX,
- lineage and natural-history presentation,
- accessibility and readability,
- audio and atmosphere,
- performance scaling,
- additional ecosystem breadth,
- release-facing polish.

The order of development is therefore:

```text
Depth
  ↓
Ecological Stability
  ↓
Observability
  ↓
11-Slice Completion Test
  ↓
Vivarium Alpha
  ↓
Broader Content / Final Product
```

The goal is not to build the ecosystem with the most features.

The goal is to reach the point where a player can watch the same limited set of species for a long time because they genuinely want to know **what happens next**.

---

# Roadmap principle

Later slices may refine earlier systems, but they should not create circular architectural dependencies.

The intended dependency flow is broadly:

```text
World Foundation
      ↓
Survival Loop
      ↓
Reproduction / Genetics
      ↓
Ecological Stability
      ↓
Food Web
      ↓
Population Behaviour
      ↓
Climate / Deep Time
      ↓
Observer Tools
      ↓
History / Lineage
      ↓
Final Presentation
      ↓
Scale / Release
```

The visual layer is deliberately late because the ecosystem should remain valuable even when run headless.
