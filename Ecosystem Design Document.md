# 🐾 LivingSim: Advanced 2D Ecosystem Design Document (Detailed Implementation Blueprint)

Project Vision: A deterministic, high-performance 2D ecosystem simulation supporting 1,000–5,000 concurrent animal entities with emergent predator–prey, evolutionary, and climate dynamics.

Role: The Observer. No intervention mechanics. The simulation must run identically headless.

Core Principle: Scale through simplicity. Depth through interaction frequency.

## 1. 🏗️ Core Architecture (Implementation-Level Detail)

### 1.1 Deterministic Simulation Core

Fixed Tick Model: Fixed timestep (e.g., 20–60 ticks per simulated day).

No variable delta time.

No real-time clock dependency.

Global Systems Order (Strict Execution Order):

ClimateSystem

BiomeRegenerationSystem

MetabolismSystem

PerceptionSystem

DecisionSystem

MovementSystem

CombatSystem

FeedingSystem

ReproductionSystem

LifecycleSystem

CarcassSystem

SpatialPartitionUpdateSystem

Note: Order must never change to preserve determinism.

### 1.2 ECS Data Layout (Performance-Oriented)

All components should be structs stored in tightly packed arrays, indexed by EntityID. Avoid virtual calls, object references between entities, and dynamic resizing during the tick loop.

Required Components:

TransformComponent (float X, float Y, float VX, float VY)

MetabolismComponent (float Energy, float Hunger, float MaxEnergy, float MetabolismRate)

GeneticsComponent (float Speed, float VisionRadius, float AttackPower, float Defence, float Fertility, float MutationRate)

LifecycleComponent (int AgeTicks, LifecycleStage Stage)

CombatComponent (float CurrentHealth, float MaxHealth)

DietComponent (DietType enum)

GroupComponent (`int` GroupID)

### 1.3 Spatial Partitioning (Critical for Scaling)

World Layout: Fixed-size grid (e.g., 256x256 tiles).

Chunk Size: 8x8 or 16x16 tiles.

Chunk Data: Each chunk maintains `List<int>` AnimalIndices and `List<int>` CarcassIndices.

Scanning: During a scan, only the current chunk and its 8 neighbouring chunks are checked, reducing distance check complexity from $O(N^2)$ to $O(N)$.

## 2. 🌱 Phase 1 — Core Survival Implementation

### 2.1 Tile Data Model

Each WorldCell contains:

TerrainType

float PlantBiomass

float BaseRegenRate

float DegradationLevel

bool HasWater

Biomass Regeneration Formula:
`EffectiveRegen = BaseRegenRate * SeasonModifier * (1 - DegradationLevel)`
(Clamp PlantBiomass between 0 and MaxBiomass).

### 2.2 Energy Model

Energy acts as the universal survival currency.
Per Tick Energy Drain:
Energy -= MetabolismRate + (Speed * MovementCostMultiplier)
If Energy <= 0, the entity dies.

### 2.3 Perception Model

For each entity:

Determine scan radius (VisionRadius).

Query the spatial partition.

Filter entities: Predator? Prey? Mate? Same species?
(No memory storage. All decisions are based on the immediate, observable state).

### 2.4 Decision Priority Hierarchy

Order of needs:

Flee: If a predator is within threat radius.

Seek Food: If Energy < HungerThreshold.

Seek Mate: If Energy > ReproductionThreshold.

Wander: Small random vector perturbation to avoid static clustering.

## 3. 🧬 Phase 2 — Genetics & Evolution Mechanics

### 3.1 Reproduction Conditions

To reproduce, an entity must:

Have Stage == Adult

Have Energy > ReproductionThreshold

Have a mate within proximity

Pass a random check against Fertility
(Energy cost is applied upon success).

### 3.2 Offspring Generation

For each trait:
ChildTrait = ((ParentA + ParentB) / 2) + Random(-MutationRate, MutationRate)
(Clamp traits to hard species limits to prevent physics-breaking speeds).

### 3.3 Evolutionary Feedback (Emergent)

Faster predator -> more successful hunts.

More hunts -> higher survival.

Higher survival -> more offspring.

Trait average increases over generations.
(No global species tracking required. Evolution emerges purely from reproduction bias).

### 3.4 Lifecycle Transitions

Stages determined by AgeTicks:

Juvenile (0–J1): Lower stats, consumes less, cannot reproduce.

Adult (J1–J2): Baseline stats, fully capable.

Elder (J2+): Gradual speed/defence decay, fertility reduction.

## 4. 🐺 Phase 3 — Social Mechanics

### 4.1 Group ID Assignment

At each tick:

If an animal is near others of the same species within GroupRadius, assign the same GroupID.

If isolated, reset to a unique ID.
(Group IDs are recalculated dynamically; there is no persistent leadership tracking).

### 4.2 Flocking Steering Vectors

The movement vector is composed of:

Cohesion: Steer towards the group's centre of mass.

Separation: Avoid overlapping with neighbours.

Alignment: Match the average velocity of the group.

Target Attraction: Steer towards food or prey.

Predator Repulsion: Steer directly away from threats.
(Final velocity is a weighted sum of these vectors. Weights are configurable in SimConfig).

### 4.3 Combat Resolution

When within attack range, calculate damage:
Damage = (AttackPower * Random(0.8, 1.2)) - Target.Defence
If Health <= 0, trigger the death event and spawn a carcass.

## 5. 🌍 Phase 4 — Food Web Dynamics

### 5.1 Carcass Lifecycle

On death, replace the entity with a CarcassComponent containing an EnergyValue and a DecayTimer.
Per tick:
DecayTimer--
If DecayTimer <= 0, temporarily increase the local tile's BaseRegenRate and remove the carcass.

### 5.2 Dietary Behaviour

Herbivores: Seek the tile with the highest biomass in their scan radius.

Carnivores: Prefer injured prey. Prefer carcasses over live prey if energy is critically low.

Scavengers: Prioritise carcasses above all else.

## 6. ⏳ Phase 5 — Long-Term Ecological Pressure

### 6.1 Climate Oscillation Model

Global variable ClimatePhase oscillates between [-1.0, +1.0] very slowly over thousands of ticks (via a sine function or random drift).
Applied as: `SeasonModifier = BaseSeason * (1 + (ClimatePhase * ClimateImpactFactor))`

### 6.2 Drought Consequences

When ClimatePhase is low:

Lower biomass regeneration -> increased starvation.

Predators overhunt dying prey -> population crash.

Survivors tend to be those with low-metabolism genotypes.

### 6.3 Habitat Degradation Logic

If PlantBiomass == 0 for longer than OvergrazingThreshold, then DegradationLevel += DegradationRate.
Recovery: When biomass naturally climbs above a RecoveryThreshold, gradually reduce DegradationLevel.

## 7. 📈 Balancing & Scaling Targets

### 7.1 Performance Goals

60 FPS (if visualised) at 2,000+ entities.

Headless mode capable of simulating 10,000 ticks per second.

Zero Garbage Collection (GC) spikes during the simulation tick.

### 7.2 Ecological Stability Goals

Within 50,000 ticks:

Predator–prey oscillations (Lotka-Volterra dynamics) are clearly visible.

At least one population crash and recovery occurs.

Trait averages shift measurably in response to the environment.

## 8. 🔬 What Is Intentionally Excluded

To preserve massive scalability, the following are strictly forbidden:

No per-plant entities (use numerical biomass).

No soil chemistry or sub-surface water modelling.

No detailed disease vector modelling.

No territorial border painting/memory systems.

No memory or learning AI (GOAP/Behavior Trees are replaced by immediate vector steering).

No permanent biome rewriting.

Final Design Philosophy

The ecosystem must feel alive, unpredictable, self-correcting, and biologically plausible, while remaining strictly deterministic, numerically driven, allocation-free, and architecturally clean.

In a 2D simulation, realism is not derived from microscopic fidelity, but from relentless systemic interaction across thousands of entities over deep time.
