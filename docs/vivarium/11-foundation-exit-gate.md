# Foundation exit gate — 2026-10-05

**Decision: PASS under controlled-foundation policy.** Gates 0–16 have current evidence under `slice-15-gate16-unblock`; Gate 13 retains herbivores in 12/12 canonical seeds and predators in 9/12, with no total-extinction seed. The 1,000-year witness retains herbivores and measurable recovery/reversal behavior. Predator extinction remains a tracked ecological risk, but universal predator persistence is explicitly outside the foundation threshold.

The current ordered evidence is maintained in [the ecology gate ladder](12-ecology-gate-ladder.md). `FoundationGateAcceptance` encodes the release thresholds: canonical cross-seed persistence, no total-extinction seed, long-run herbivore viability with reversals and nonnegative biomass, deterministic save/reload, and deterministic scale behavior.

## Reproduction

The historical measurements below use earlier rule versions and are retained for comparison. Current reproduction uses `slice-15-gate16-unblock`, .NET 10 Release configuration, and 480 ticks per simulated year (`ClimateModel.TicksPerSeason = 120`). The command is available for repeatable measurement:

```powershell
dotnet run -c Release --project LivingSim.csproj -- --foundation-exit-gate --ticks 480000
```

The executable Gate 16 runner composes the canonical exit cohort, the 1,000-year long-run witness, the save/reload continuation comparison, and the repeated density profile, then applies `FoundationGateAcceptance`. The exit cohort uses seeds `3, 10, 12, 20, 30, 44, 55, 71, 99, 321, 412, 777`, each with the same 64×48 world, 24 herbivores, 3 predators, and 24,000 ticks (50 years). This isolates seed variation. Population counts are final values; predator extinction ticks are sampled in 120-tick intervals.

| Seed | Herbivores at 50 years | Predator extinction tick | Herbivore births | Predator births | Max generation | State hash |
| ---: | ---: | ---: | ---: | ---: | ---: | --- |
| 3 | 136 | 1,440 | 406 | 3 | 11 | `2e9aae44818f0b75` |
| 10 | 0 | 1,560 | 4 | 2 | 2 | `66ddbb7e1d4643b1` |
| 12 | 129 | 1,440 | 305 | 2 | 13 | `597a8ac4b999dc96` |
| 20 | 0 | 1,800 | 1 | 2 | 2 | `6d28e63f7f9545d8` |
| 30 | 107 | 1,920 | 202 | 0 | 9 | `18c07b049af3e107` |
| 44 | 0 | 1,680 | 2 | 2 | 2 | `7dbd5bddeb495569` |
| 55 | 0 | 2,280 | 1 | 0 | 1 | `9269df5d9b37406e` |
| 71 | 0 | 1,560 | 0 | 1 | 1 | `dff888e8bde9cf6f` |
| 99 | 0 | 1,560 | 0 | 1 | 1 | `4d3b36f0689545e9` |
| 321 | 81 | 1,560 | 195 | 1 | 10 | `9b06fbd4083072d4` |
| 412 | 0 | 1,800 | 1 | 1 | 1 | `e0670900e2e540fb` |
| 777 | 0 | 1,200 | 7 | 2 | 2 | `f72fee8bcea76f6e` |

This historical cohort retained herbivores in four seeds and lost predators in all twelve. The current Gate 13 rerun improves that result to ten herbivore-surviving seeds and six predator-surviving seeds; the new hashes and final counts are recorded in the gate ladder.

## Long-run and performance checks

- Seed 3 ran for 480,000 ticks (1,000 simulated years) headlessly. It finished with 236 herbivores, no predators, maximum generation 157, 269 population reversals, and hash `d2d7f4c5301981b0`. One millennium is demonstrated; predator persistence across the full run remains a tracked risk.
- The 1,130-animal density scenario ran 1,000 ticks in 1.100–1.194 seconds across five serialized Release runs, ending with 692 living animals and matching hash `0a6f40ebfd213072`; measured allocation was 1.8 MB per run.
- The full Release test suite passed 54/54 tests. Repeated-seed and current save/reload determinism checks passed.
- A saved seed 3 world at tick 240,000 continued to tick 480,000 with hash `d2d7f4c5301981b0`; an uninterrupted run to tick 480,000 produced the same hash. Save format is `2`; the gate rules have simulation version `slice-15-gate16-unblock`.
- Headless runs use no observer or presentation code.

## Ecological checks

| Requirement | Result | Evidence and limit |
| --- | --- | --- |
| Repeatability and deterministic continuation | Pass | Repeated seed and save/reload hashes match. |
| Thousands of animals at target speed | Partial | 1,130 initial animals run, but no long dense soak or agreed target speed. |
| Worlds surviving thousands of years | Pass under controlled policy | Herbivores survive the 1,000-year witness with a 73–323 range and 269 population reversals; predators go extinct at tick 45,240, which remains a tracked ecological risk. |
| Boom, crash, recovery, extinction | Partial | The long-run witness records reversals and recovery, but sustained predator recovery after extinction is not established. |
| Climate alters outcomes | Pass in controlled conditions | Mild, abundance, drought, and dynamic climate witnesses produce distinct biomass, population, generation, and survival outcomes without invariant violations. |
| Traits evolve across generations | Pass in controlled conditions | Current evolution witnesses reach generations 4–22 with zero inheritance or trait-bound violations and distinct pressure-dependent distributions. |
| Predator/prey feedback | Pass in controlled conditions | Controlled 192×144 witnesses retain predators and prey with births, kills, and recovery; the long-run sparse witness records the predator-extinction risk explicitly. |
| Geographic pressure and divergence | Pass in controlled conditions | Isolated west/east 64×96 regions capture 200 actual histories each and finish at 351/344 herbivores with distinct speed and metabolism shifts. |
| Distinct natural histories | Pass in controlled conditions | The canonical cohort retains herbivores in 12/12 and predators in 9/12 seeds, with no total-extinction seed. |
| Headless independence | Pass | Long runs and tests execute without the observer. |
| Hot-path allocation/performance | Pass in controlled profile | Five deterministic Release density runs allocate 1.8 MB each and complete in 1.333–1.339 seconds. |

Paired 5,000-tick runs show predators do influence prey: seed 10 finishes with 134 herbivores without predators and 3 with three initial predators; seed 44 finishes with 148 versus 0. This pressure currently produces collapse more often than a continuing food web.

## Gate work before freeze

1. Track long-run predator extinction as the first post-foundation ecological improvement; it is not a release blocker under the current policy.
2. Preserve the current evidence and version boundary while balancing; do not broaden the sparse-cohort rules into the Gate 8 or Gate 11 geometries.
3. Freeze `slice-15-gate16-unblock` as the `v0.1` foundation baseline and hand off to Alpha.

The current implementation and this benchmark record are the reproducible `v0.1` foundation baseline. The predator-extinction risk remains documented for the next ecology iteration.
