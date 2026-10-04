# 07 - Development Roadmap

The roadmap is ordered so every slice produces a testable simulation improvement while avoiding dependency on later presentation decisions.

Each slice should be completed and stabilised before systems that depend on it are added.

---

## Slice 0 - Preserve the prototype

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
