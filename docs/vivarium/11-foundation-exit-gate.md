# Foundation exit gate — 2026-10-05

**Decision: FAILED. Do not tag or announce `LivingSim-Eco v0.1 — Simulation Foundation` yet.** The 11 slices implement the planned features, but the ecology does not yet pass the product gate. The dominant outcome is predator extinction, often followed by total animal extinction. The evidence does not support stable predator/prey cycles, climate-driven population stories, or geographically diverged lineages.

## Reproduction

The measurements below use `slice-11-gate1` rules, .NET 10 Release configuration, and 480 ticks per simulated year (`ClimateModel.TicksPerSeason = 120`). The command is available for repeatable measurement:

```powershell
dotnet run -c Release --project LivingSim.csproj -- --foundation-gate --seed 3 --width 64 --height 48 --ticks 24000 --herbivores 24 --predators 3
```

The exit cohort used seeds `3, 10, 12, 20, 30, 44, 55, 71, 99, 321, 412, 777`, each with the same 64×48 world, 24 herbivores, 3 predators, and 24,000 ticks (50 years). This isolates seed variation. Population counts are final values; predator extinction ticks are sampled in 120-tick intervals.

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

Four seeds retained herbivores at year 50; eight lost every animal. Every seed lost predators by tick 2,280 (4.75 years). The surviving populations differ in size and traits, yet their food-web stories converge after predators disappear.

## Long-run and performance checks

- Seed 3 ran for 480,000 ticks (1,000 simulated years) headlessly in 90.68 seconds. It finished with 145 herbivores, no predators, 8,562 herbivore births, maximum generation 102, and hash `74d14122428a83a5`. One millennium is demonstrated; multi-millennium survival across worlds is not.
- The 1,130-animal density scenario ran 1,000 ticks in about 1.7–1.9 seconds in Release, ending with 775 living animals and hash `34f34192f7727943`. Measured current-thread allocation varied from 52.6 to 110.6 MiB across runs. Allocation needs a controlled profile before claiming the hot path is clean.
- The full test suite passed 40/40 tests. Seed 3 repeated at 24,000 ticks produced the same hash `2e9aae44818f0b75`.
- A saved seed 3 world at tick 2,500 continued to tick 5,000 with hash `6bfb14a24414dc07`; an uninterrupted run to tick 5,000 produced the same hash. Save format is `1`; the gate rules have simulation version `slice-11-gate1`.
- Headless runs use no observer or presentation code.

## Ecological checks

| Requirement | Result | Evidence and limit |
| --- | --- | --- |
| Repeatability and deterministic continuation | Pass | Repeated seed and save/reload hashes match. |
| Thousands of animals at target speed | Partial | 1,130 initial animals run, but no long dense soak or agreed target speed. |
| Worlds surviving thousands of years | Partial | One seed survived 1,000 years; broader multi-millennium evidence is missing. |
| Boom, crash, recovery, extinction | Partial | Boom and extinction occur; sustained recovery after ecological collapse is not established. |
| Climate alters outcomes | Unproven | Climate modifiers and eras run, but a controlled ecological comparison has not isolated their effect. |
| Traits evolve across generations | Partial | Seed 3 reached generation 102; mean herbivore metabolism changed from 1.62 to 1.23 over 1,000 years. Causation and local divergence remain unproven. |
| Predator/prey feedback | Failed | Predators alter prey outcomes, but every predator population dies within five years. |
| Geographic pressure and divergence | Unproven | Different maps and habitats exist; no measured isolated-lineage divergence. |
| Distinct natural histories | Failed | Final herbivore counts differ, but all worlds lose predators early. |
| Headless independence | Pass | Long runs and tests execute without the observer. |
| Hot-path allocation/performance | Partial | Density throughput is promising; allocation measurement needs controlled profiling. |

Paired 5,000-tick runs show predators do influence prey: seed 10 finishes with 134 herbivores without predators and 3 with three initial predators; seed 44 finishes with 148 versus 0. This pressure currently produces collapse more often than a continuing food web.

## Gate work before freeze

1. Rebalance hunting, prey escape, carcass energy, and predator reproduction as one measured system. A focused correction already stopped packs overriding prey targets with plant migration and removed plant habitat as a carnivore breeding requirement; it extended predator persistence but did not clear the gate.
2. Run controlled climate and geographic comparisons, including local trait frequencies and separated populations. Establish whether selection pressure rather than random drift explains the observed trait changes.
3. Profile allocation and CPU after warm-up at sustained density, and test multiple worlds for more than 1,000 simulated years.
4. Re-run the same seed cohort and require at least some long-lived predator/prey cycles, collapses with recovery, climate bottlenecks, and geographically distinct trajectories before assigning `v0.1`.

The current implementation and this benchmark record should be kept as a reproducible pre-freeze baseline. No `v0.1` tag or simulation rule freeze has been created.
