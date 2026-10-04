# LivingSim-Eco

LivingSim-Eco is an artificial-life ecosystem project focused on autonomous nature simulation, long-term ecological change, and evolution.

The original project was a console/ASCII ecosystem prototype in C#. The current direction is a broader **observer-first vivarium**: generate a deterministic world from a seed, allow the ecosystem to develop without direct player intervention, and watch natural history emerge across individuals, populations, generations, and deep time.

> The final visual style and rendering technology are intentionally not locked yet.

## Current design documentation

The Vivarium concept and development roadmap are broken down under [`docs/vivarium/`](docs/vivarium/README.md):

- [Product Vision](docs/vivarium/01-product-vision.md)
- [Player Role & Experience](docs/vivarium/02-player-role-and-experience.md)
- [Simulation & Evolution](docs/vivarium/03-simulation-and-evolution.md)
- [History, Lineages & Seeds](docs/vivarium/04-history-lineages-and-seeds.md)
- [Presentation Direction](docs/vivarium/05-presentation-direction.md)
- [Technical Architecture](docs/vivarium/06-technical-architecture.md)
- [Development Roadmap](docs/vivarium/07-development-roadmap.md)
- [Locked Decisions & Open Questions](docs/vivarium/08-decisions-and-open-questions.md)

## Core direction

- Observer-first rather than ecosystem management.
- Seed-driven deterministic worlds.
- Autonomous predator/prey and food-web simulation.
- Inheritance, mutation, and readable evolution.
- Deep-time climate and ecological pressure.
- Population, lineage, and world-history observation.
- Simulation core capable of running headless and independently from presentation.

## Current runnable foundation

Slice 1 provides a deterministic, animal-free world with terrain, biomes, numeric plant biomass, and resource regeneration. See [RUN.md](RUN.md) for headless command-line usage and verification.

## Legacy prototype

The earlier console prototype included animals that moved, hunted, reproduced, interacted with resources, and reacted to day/night and seasonal changes. Its implementation is preserved at the `legacy-ascii-prototype` Git tag and documented in the [Legacy Prototype Baseline](docs/vivarium/00-legacy-prototype-baseline.md). It serves as behavioural reference material for the rewrite.

The rewrite should preserve the useful ecological ideas without treating the legacy architecture as the final technical foundation.
