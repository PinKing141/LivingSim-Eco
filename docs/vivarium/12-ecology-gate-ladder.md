# Ecology gate ladder — run in order

Status on 2026-10-05: **Gates 0–5 pass under `slice-12-gate5`; work continues at Gate 6.** Gates 6–16 cannot be claimed current-green. The foundation has **not** passed, and no `v0.1` tag or rule freeze is justified. The previous 12-seed exit cohort in [11-foundation-exit-gate.md](11-foundation-exit-gate.md) remains the fixed final cohort; the controlled seeds below are diagnostics, not replacements.

The rule for this ladder is to stop at the first lower failing gate. A simulation rule change requires Gate 0, then all previously cleared ecological gates, to be rerun before moving higher. One simulated year is 480 ticks. The current rule version is `slice-12-gate5`; save format is 2. Opt-in hunting and resource telemetry is diagnostic and is not included in the state hash.

| Gate | Status | Evidence |
| --- | --- | --- |
| 0 Engineering regression | Pass | 47/47 Release tests; repeated-seed and save/reload hash tests; headless execution; invariant validation; density benchmark 1,130 animals/1,000 ticks in 1.713 s under `slice-12-gate5`. Original baseline was ~1.7–1.9 s. Allocation remains noisy and needs controlled profiling at Gate 15. |
| 1 Prey-only viability | Pass | Six fixed seeds (3, 10, 12, 44, 71, 99), each 100 years in mild constant climate, 24 founders, no predators. All survived at 165–195 herbivores with 970–1,128 births, 792–884 offspring reaching maturity, generations 17–22, starvation, local biomass depletion and recovery. |
| 2 Predator basic survival | Pass in controlled conditions | One representative size-3/vision-6/speed-2/metabolism-1 predator, 160 initial prey, no predator mate, mild climate, 6,000 ticks. All six fixed seeds survived, with 52–66 kills, 296–307 meals, no starvation, and carcass gain of 7,059–7,219 energy versus 6,000 upkeep. |
| 3 Hunt economics | Pass in controlled conditions | The same six runs recorded 67–76 pursuits, 6–14 abandoned pursuits, and 106–133 attacks. Failed pursuits cost time and ongoing metabolism. The model currently has **zero additional per-step movement or combat energy charges**; the telemetry reports those as zero. Carcass gain exceeds metabolism, though satiation leaves much carrion to decay. Bite-level attribution to kill versus natural-death carcasses remains uninstrumented. |
| 4 Predator reproduction | Pass in controlled conditions | In the favourable 192×144 habitat with 1,440 prey and eight age-staggered, compatible founders, seed 3 produced 82 births and ended with 5 predators; seed 10 produced 61 births and ended with 5 predators. Both produced mature offspring through the final year. |
| 5 Controlled predator–prey viability | Pass in controlled conditions | Same 100-year configuration, current `slice-12-gate5` rules. Seed 3: predators 8→5, peak 40, prey 1,440→1,491 with a low of 1,167, 82 births, 3,779 kills, no extinction. Seed 10: predators 8→5, peak 31, prey 1,440→1,502 with a low of 1,175, 61 births, 3,335 kills, no extinction. Both show predator births, predation, prey decline and recovery, and mature recruitment. These are controlled witnesses, not cross-seed evidence. |
| 6–8 | Historical evidence only | Earlier `slice-11-gate8` rules had a 100-year predator–prey recovery witness, resource feedback, and multi-species interactions. Those results are preserved below but are **not current passes** after inheritance, satiation and mate-seeking changes. Gate 6 is the next required current run. |
| 9–16 | Not current-run | An initial Gate 9 probe before the latest changes exposed downward-biased inheritance and near-universal low-trait convergence. The bias was fixed, but current Gate 9 cannot be rerun until Gate 5 is green. Gates 10–16 have not been run. |

The Gate 5 recovery fix is four focused rule changes: predator offspring receive the species starting-energy reserve; predators must be fully satiated before breeding; carcass-fed predators have the measured lower metabolic-cost divisor; and prey search expands only when four or fewer predators remain. The latter avoids the early over-predation caused by a globally wider search radius. The old Gate 5 failure retained more than 1,400 prey after predator extinction; the current witnesses avoid that extinction while retaining prey declines and recovery. The reproduction regression fixture now uses 2,000 energy to represent the new 1,800-energy breeding reserve. The next experiment is Gate 6 recovery validation under these unchanged rules.

Gate 8 exposed two rule defects: omnivores could select themselves as prey because they are both hunters and plant-eaters, and scavengers were charged herbivore-level metabolism despite relying on intermittent carrion. Both were corrected and regression-tested before Gate 8 was repeated. The expanded cohort still loses omnivores and scavengers, which is an ecological warning rather than grounds to require every species to survive.

Historical `slice-11-gate8` evidence: seed 3's 192×144, eight-founder predator/prey pair had 4 predators, 1,803 prey, 123 predator births and generation 11 at year 100. Predators fell from 13 to 3 and recovered to 6; prey fell to 1,170 by year 26 and recovered above 1,500. Biomass fell to 4.96 million by year 3 and recovered to 7.66 million by year 25. An expanded food web at year 25 retained base herbivores, large and small herbivores, ordinary and apex predators, while omnivores and scavengers went extinct. These are reproducible historical observations, **not passes for `slice-11-gate11`**.

## Reproduce the current checks

```powershell
dotnet test -c Release --no-restore
dotnet run -c Release --no-build -- --benchmark density --ticks 1000
dotnet run -c Release --no-build -- --prey-gate --seed 3 --ticks 48000 --herbivores 24 --width 64 --height 48
dotnet run -c Release --no-build -- --predator-gate --seed 3 --ticks 6000 --herbivores 160 --width 64 --height 48
dotnet run -c Release --no-build -- --predator-reproduction-gate --seed 3 --ticks 48000 --herbivores 1440 --predators 8 --width 192 --height 144
dotnet run -c Release --no-build -- --predator-reproduction-gate --seed 10 --ticks 48000 --herbivores 1440 --predators 8 --width 192 --height 144
```

The controlled `--predator-reproduction-gate` founder traits, sex balance and staggered ages are deliberate. They isolate predator demographic viability from a random founder's niche mismatch and synchronized founder ageing; they do not modify ordinary world initialization. The runner samples annual predator and prey counts, age/sex structure, cumulative births and kills, nearby eligible prey, biomass by quadrant, plant consumption and regeneration, herbivore starvation and predator outcomes by inherited size.
