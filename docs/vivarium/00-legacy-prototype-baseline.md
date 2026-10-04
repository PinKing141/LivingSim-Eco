# 00 - Legacy Prototype Baseline

## Purpose

The original console/ASCII simulator is preserved as behavioural reference material for the Vivarium rewrite. It is not the technical foundation for the new simulation.

The preserved snapshot is Git tag `legacy-ascii-prototype`, which points to commit `59480ec` (`Add ecosystem design doc; overhaul arch doc`). This is the last complete commit before `dcbca7e` removed the legacy simulation modules.

To inspect the preserved implementation without altering the rewrite branch:

```powershell
git show legacy-ascii-prototype:Program.cs
git switch --detach legacy-ascii-prototype
```

Return to the rewrite with:

```powershell
git switch main
```

## Behaviours worth preserving as reference

The prototype demonstrated several useful ecosystem-facing behaviours:

- Procedural terrain and biome generation.
- Plants/resources that replenish over time.
- Herbivore, carnivore, and omnivore roles.
- Movement, local interaction, feeding, hunting, ageing, energy loss, health, death, and reproduction.
- Day/night and seasonal environmental change.
- A deterministic configuration seed and deterministic ID generation.
- Population metrics and short-term population history.
- An ASCII visualisation with pause, speed, and statistics controls.

These are references for product behaviour only. The rewrite should validate which of them materially support the observer-first vivarium, rather than reimplementing them blindly.

## Deliberately not carried forward as architecture

The rewrite must not depend on the following legacy implementation patterns:

- Simulation flow coupled directly to `Console` input, drawing, and `Thread.Sleep`.
- Rendering performed as part of the main simulation loop.
- The legacy `Animal`/`AnimalManager` object model as the scaling model for future populations.
- `System.Random` as the only determinism boundary for the whole system.
- The existing grid, world-generation, visualisation, and metrics classes as fixed APIs.
- Legacy scents, dens, short-term memory, territorial behaviour, and social logic as assumed baseline features.
- The legacy three-species roster as the required first milestone; Slice 2 deliberately starts with one herbivore and one predator.
- Terminal/ASCII output as a permanent presentation commitment.

## Authority and comparison rule

`main` is the authoritative rewrite line. The `legacy-ascii-prototype` tag is read-only reference material for comparing observable behaviours, debugging regressions, and recovering historical context.

Changes to the rewrite should follow the Vivarium technical architecture and 11-slice roadmap, even when they differ from the legacy implementation.
