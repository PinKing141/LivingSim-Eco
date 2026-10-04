# 03 - Simulation & Evolution

## Simulation vision

LivingSim should create an ecosystem that is simple enough to simulate at scale but rich enough for complex behaviour to emerge from interaction.

The project should favour **systemic pressure and ecological trade-offs** over microscopic biological detail.

## World layer

The world should eventually support:

- a grid-based 2D environment,
- generated terrain and biomes,
- numeric plant biomass rather than thousands of individual plant entities,
- water availability where it meaningfully affects survival,
- terrain accessibility and movement cost,
- seasonal resource pressure,
- later deep-time climate variation,
- temporary local habitat degradation and recovery.

The environment should affect survival strongly enough that geography matters to evolution.

## Animal layer

Animals should be defined through reusable data and systems rather than bespoke behaviour scripts whenever possible.

Core data may include:

- species identity,
- ecological role,
- position,
- velocity or movement state,
- sensory range,
- energy / hunger,
- health,
- age,
- lifecycle stage,
- reproduction state,
- inheritable traits.

Potential ecological roles include:

- herbivore,
- carnivore,
- omnivore,
- scavenger.

## Core behaviour hierarchy

The exact AI model is still open, but the baseline behaviour should stay understandable and cheap to simulate.

A simple priority model may be:

1. flee immediate threat,
2. seek critical food,
3. seek critical water if retained as a system,
4. seek reproductive opportunity,
5. wander / explore.

Additional social, territorial, or migration behaviour should only be added when it creates meaningful ecological outcomes.

## Survival loop

The ecosystem should include:

- metabolism and energy loss,
- movement cost,
- feeding,
- predation,
- combat where needed,
- ageing,
- death,
- carcasses returning energy to the food web,
- reproduction,
- inheritance and mutation.

## Evolution as a core feature

Evolution is one of the main attractions of the game, not a hidden background calculation.

Traits should change because animals with advantageous combinations survive and reproduce more successfully under local conditions.

Useful candidate traits include:

- speed,
- movement efficiency,
- vision / sensory range,
- metabolism,
- body size,
- attack power,
- defence,
- fertility,
- other traits only when they create meaningful trade-offs.

## Trait design rule

Every inheritable trait should have a cost or limitation.

Examples:

- higher speed may increase energy use,
- larger size may improve combat but require more food,
- greater vision may increase sensory/metabolic cost,
- high fertility may produce more offspring but increase reproductive cost.

Without trade-offs, evolution risks becoming a simple march toward globally maximum stats.

## Inheritance

The final inheritance model is not locked.

The first implementation should be intentionally simple, for example:

`child trait = parent average + bounded mutation`

The important outcome is that natural selection can produce visible long-term changes without requiring a full genetic simulation.

## Lifecycle

A minimal lifecycle can use:

- juvenile,
- adult,
- elder.

The simulation does not initially need detailed pregnancy, parenting, teaching, or developmental AI unless those systems later prove important to the observed ecology.

## Population dynamics to target

Healthy simulation behaviour should be capable of producing:

- predator-prey oscillation,
- resource-driven population booms,
- starvation waves,
- population crashes,
- recovery,
- local extinction,
- recolonisation,
- geographic divergence,
- measurable trait selection over deep time.

These outcomes should emerge rather than be directly scripted.
