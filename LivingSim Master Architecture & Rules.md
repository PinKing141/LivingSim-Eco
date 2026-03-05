```md
# 🐾 LivingSim: Advanced 2D Ecosystem Design Document (Optimised & Finalised)

**Project Vision:** A highly performant, emergent 2D animal ecosystem simulation capable of running thousands of entities deterministically.

**Role:** The Observer (No intervention. The simulation must run headless.)

**Core Principle:**  
*Complexity emerges from interaction density, not biological micromanagement.*

This document refines and constrains the design to ensure scalability, clarity, and architectural discipline.

---

# 1. 🏗️ Core Architecture & Simulation Law

To maintain 1,000–5,000 concurrent entities without instability or frame stutter, the following constraints are mandatory.

---

## 1.1 Simulation Model

### Entity-Component-System (ECS)

- **Entities** = ID + pure data.
- **Components** = struct-like data containers.
- **Systems** = stateless processors operating on filtered component sets.

Example components:
- `Position`
- `Velocity`
- `Metabolism`
- `Needs`
- `Genetics`
- `CombatStats`
- `Lifecycle`

Systems:
- `MetabolismSystem`
- `PerceptionSystem`
- `MovementSystem`
- `CombatSystem`
- `ReproductionSystem`
- `CarcassSystem`
- `ClimateSystem`

No logic inside components. No cross-system references.

---

## 1.2 Performance Mandate

### Deterministic Simulation
- Single `WorldRandom` seeded at world creation.
- No `DateTime.Now`.
- No system-level randomness outside seeded generator.

### Spatial Partitioning
- World divided into fixed-size chunks.
- Each chunk maintains:
  - Entity index list
  - Carcass index list

All scans are local:
- Same chunk
- Adjacent chunks only

No global scans.

---

### No Memory Systems

Animals:
- Do not store coordinates of resources.
- Do not remember attackers.
- Do not build path histories.

Behaviour is purely reactive:
- Based on current sensory radius.

---

### Navigation Model

Primary movement uses:
- Vector steering (attraction/repulsion).
- Boids-style cohesion for grouping.

A* pathfinding:
- Only triggered when target is blocked by impassable terrain.
- Limited to short-range.

No dynamic navmesh.

---

### Zero Allocation in Tick

Inside main simulation loop:
- No `new`.
- No `LINQ`.
- No temporary list creation.
- Reuse preallocated buffers.

All loops are index-based `for` loops.

---

# 2. 🌱 Phase 1 — Core Survival Loop

Goal: Stable predator–prey balance without biological overengineering.

---

## 2.1 World Grid

Each `WorldCell` contains:

- `TerrainType`
- `PlantBiomass` (float)
- `BaseRegenRate`
- `DegradationLevel`

No:
- Soil chemistry
- Mineral modelling
- Root systems
- Atmospheric gas simulation

---

## 2.2 Biomass System

Plant biomass is purely numeric.

Each tick:
```

PlantBiomass += BaseRegenRate * SeasonModifier * DegradationModifier
Clamp to MaxBiomass

```

Herbivores:
- Subtract biomass directly.
- Convert biomass to energy via efficiency ratio.

---

## 2.3 Basic Animal Loop

Each tick:

1. Drain energy (Metabolism).
2. Increase hunger.
3. Scan nearby chunks.
4. Determine highest priority:
   - Flee predator
   - Seek food
   - Seek water
   - Seek mate
5. Apply steering vector.
6. Move.
7. Resolve collisions or combat.

No complex planning system required.

---

# 3. 🧬 Phase 2 — Lightweight Genetics

Goal: Allow evolution through numeric drift.

---

## 3.1 Trait Structure

Each entity has:

- `Speed`
- `VisionRadius`
- `Metabolism`
- `AttackPower`
- `Defence`
- `Fertility`

All numeric.
No gene trees.
No dominance modelling.

---

## 3.2 Inheritance Model

Offspring trait calculation:

```

ChildTrait = (ParentA + ParentB) / 2 ± MutationOffset

```

MutationOffset:
- Small random float.
- Controlled by global mutation rate.

Emergent outcomes:
- Faster predators reproduce more.
- Efficient herbivores survive droughts.

---

## 3.3 Lifecycle

Three states only:

1. Juvenile
   - Reduced stats
   - Cannot reproduce
2. Adult
   - Full stats
3. Elder
   - Gradual stat decay
   - Increased mortality risk

No parental AI.
No teaching systems.

---

# 4. 🐺 Phase 3 — Social & Pack Dynamics

Goal: Emergent cooperation without hierarchical AI.

---

## 4.1 Radius-Based Grouping

If same-species entities are within grouping radius:
- Assign shared `GroupID`.
- Apply cohesion steering.

Group bonuses:
- Flat defence bonus.
- Flat attack bonus.
- Lower flee threshold.

No:
- Alpha ranking
- Political modelling
- Leadership AI

---

## 4.2 Emergent Pack Hunting

During combat calculation:

If:
```

PredatorLocalCount > PreyLocalCount

```

Apply:
```

AttackPower *= PackMultiplier

```

Encirclement emerges naturally from cohesion + pursuit vectors.

No tactical state machines required.

---

# 5. 🌍 Phase 4 — The Trophic Web

Goal: Complete food cycle with minimal systems.

---

## 5.1 Dietary Tags

Each species has:

- Herbivore
- Carnivore
- Omnivore
- Scavenger

Interaction rules determined by tag.

---

## 5.2 Carcass Entity

On death:
- Spawn `Carcass`
- Remove animal entity

Carcass contains:
- `EnergyValue`
- `DecayTimer`

Carnivores/scavengers:
- Prefer carcass over live prey if nearby.

On full decay:
- Increase tile regeneration multiplier temporarily.

No fungi.
No insect colonies.
No nutrient chemistry.

---

# 6. ⏳ Phase 5 — Deep Time Systems

Goal: Macro change without heavy simulation layers.

---

## 6.1 Climate Oscillation

Global variable:
```

ClimateModifier

```

Over long cycles:
- Moves between drought and abundance phases.

Drought:
- Reduce biomass regeneration globally.

Abundance:
- Increase regeneration.

Simple numeric modifier only.

---

## 6.2 Trait Drift Over Generations

As selection pressures shift:
- Population averages change.
- No explicit speciation detection required.

Optional cosmetic rule:
- If trait divergence exceeds threshold, rename population variant.

No breeding compatibility logic.

---

## 6.3 Habitat Degradation

If:
```

PlantBiomass == 0 for X ticks

```

Increase:
```

DegradationLevel

```

Degradation reduces regeneration rate temporarily.

Recovery:
- Slowly decreases if biomass sustained above threshold.

No permanent terrain mutation.

---

# 7. 📊 Target Outcomes

When complete, the simulation should naturally produce:

- Predator–prey oscillations.
- Seasonal starvation waves.
- Evolution of faster predators.
- Evolution of efficient herbivores.
- Group hunting success.
- Localised extinction events.
- Population booms during abundance.

All without:

- Microscopic biological modelling.
- Speciation engines.
- Detailed climate physics.
- Individual plant entities.
- Heavy AI planners.

---

# Final Statement

The system must remain:

- Deterministic.
- Headless-capable.
- Allocation-free in hot paths.
- Numerically driven.
- Emergent through interaction density.

In 2D, depth is achieved through systemic interplay — not simulation granularity.
```
