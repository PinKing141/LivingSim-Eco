# Ecology gate ladder — run in order

Status on 2026-10-05: **stopped at Gate 5 under the current rules.** Gates 0–4 pass in controlled scenarios; Gates 6–16 cannot be claimed current-green. The foundation has **not** passed, and no `v0.1` tag or rule freeze is justified. The previous 12-seed exit cohort in [11-foundation-exit-gate.md](11-foundation-exit-gate.md) remains the fixed final cohort; the controlled seeds below are diagnostics, not replacements.

The rule for this ladder is to stop at the first lower failing gate. A simulation rule change requires Gate 0, then all previously cleared ecological gates, to be rerun before moving higher. One simulated year is 480 ticks. The current rule version is `slice-11-gate11`; save format is 2. Opt-in hunting and resource telemetry is diagnostic and is not included in the state hash.

| Gate | Status | Evidence |
| --- | --- | --- |
| 0 Engineering regression | Pass | 47/47 Release tests; repeated-seed and save/reload hash tests; headless execution; density benchmark 1,130 animals/1,000 ticks in 1.827 s after the latest rule change; invariant validation. Original baseline was ~1.7–1.9 s. Allocation remains noisy and needs controlled profiling at Gate 15. |
| 1 Prey-only viability | Pass | Six fixed seeds (3, 10, 12, 44, 71, 99), each 100 years in mild constant climate, 24 founders, no predators. All survived at 165–189 herbivores with 981–1,293 births, 756–883 offspring reaching maturity, generations 21–23, starvation, local biomass depletion and recovery. |
| 2 Predator basic survival | Pass in controlled conditions | One representative size-3/vision-6/speed-2/metabolism-1 predator, 160 initial prey, no predator mate, mild climate, 6,000 ticks. All six fixed seeds survived, with 80–96 kills, 548–564 meals, no starvation, and carcass gain of 12,885–13,211 energy versus 12,000 upkeep. |
| 3 Hunt economics | Pass in controlled conditions | The same six runs recorded 93–106 pursuits, 7–16 abandoned pursuits, and 163–197 attacks. Failed pursuits cost time and ongoing metabolism. The model currently has **zero additional per-step movement or combat energy charges**; the telemetry reports those as zero. Carcass gain exceeds metabolism, though satiation leaves much carrion to decay. Bite-level attribution to kill versus natural-death carcasses remains uninstrumented. |
| 4 Predator reproduction | Qualified pass | In seed 3's favourable 192×144 habitat with 1,440 prey and eight age-staggered, compatible founders, there were 101 births, 75 offspring reaching maturity, maximum generation 8 and a peak of 30 predators. Replacement occurs in favourable periods, but it is not durable. |
| 5 Controlled predator–prey viability | **Fail** | Under the same 100-year configuration, seed 3 lost all predators at tick 35,880 (year 74.75) with 1,470 prey remaining; seed 10 lost them at tick 17,520 (year 36.5) with 1,465 prey remaining. Seed 3's births ceased at tick 19,816 after females disappeared; seed 10's births ceased at tick 6,000 after males disappeared. Mate-seeking increased early births but did not prevent population bottlenecks and extinction. |
| 6–8 | Historical evidence only | Earlier `slice-11-gate8` rules had a 100-year predator–prey recovery witness, resource feedback, and multi-species interactions. Those results are preserved below but are **not current passes** after inheritance, satiation and mate-seeking changes. Do not proceed until Gate 5 passes again. |
| 9–16 | Not current-run | An initial Gate 9 probe before the latest changes exposed downward-biased inheritance and near-universal low-trait convergence. The bias was fixed, but current Gate 9 cannot be rerun until Gate 5 is green. Gates 10–16 have not been run. |

The early hunt failure was traced to abandoning partly eaten carcasses above an energy threshold. Hungry predators now finish nearby carcasses before seeking another hunt, while satiated predators leave them for others. The first fix changed the controlled Gate 2 result from one starvation in six seeds to six survivors; the newer satiation rule also passes Gate 2. Regression tests protect both behaviors. The current Gate 5 failure is not global prey scarcity: seed 3 and seed 10 each retain more than 1,400 prey after predator extinction. Their population trajectories show a boom followed by a sex/age bottleneck. A larger, earlier-version controlled witness survived 100 years, but that success was invalidated by later inheritance changes and cannot be counted toward the current gate.

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
