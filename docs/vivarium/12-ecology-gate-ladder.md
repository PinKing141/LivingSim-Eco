# Ecology gate ladder — run in order

Status on 2026-10-05: **stopped at Gate 5**. Gates 6–16 have not been run. The foundation has **not** passed, and no `v0.1` tag or rule freeze is justified. The previous 12-seed exit cohort in [11-foundation-exit-gate.md](11-foundation-exit-gate.md) remains the fixed final cohort; the controlled seeds below are diagnostics, not replacements.

The rule for this ladder is to stop at the first lower failing gate. A simulation rule change requires Gate 0, then all previously cleared ecological gates, to be rerun before moving higher. One simulated year is 480 ticks. The current rule version is `slice-11-gate6`; save format is 2. Opt-in hunting telemetry is diagnostic and is not included in the state hash.

| Gate | Status | Evidence |
| --- | --- | --- |
| 0 Engineering regression | Pass | 43/43 Release tests; repeated-seed and save/reload hash tests; headless execution; density benchmark 1,130 animals/1,000 ticks in 1.628–1.858 s, 32.9–95.5 MiB current-thread allocation across two runs; invariant validation. Original baseline was ~1.7–1.9 s. Allocation remains noisy and needs controlled profiling at Gate 15. |
| 1 Prey-only viability | Pass | Six fixed seeds (3, 10, 12, 44, 71, 99), each 100 years in mild constant climate, 24 founders, no predators. All survived at 203–232 herbivores, with 974–1,229 births, 835–968 offspring reaching maturity, generations 15–19, starvation, local biomass depletion and recovery. Seed 3 was rerun after the hunting change with identical hash `b494f83bd4f15d5e`. |
| 2 Predator basic survival | Pass in controlled conditions | One representative size-3/vision-6/speed-2/metabolism-1 predator, 160 initial prey, no predator mate, mild climate, 6,000 ticks. All six fixed seeds survived, with 108–134 kills, 723–840 meals, no starvation, and net carcass gain of 17,234–19,876 energy versus 12,000 upkeep. |
| 3 Hunt economics | Pass in controlled conditions | The same six runs recorded 137–154 pursuits, 15–28 abandoned pursuits, 223–274 attacks, and 16,238–17,903 distance units travelled. Carcass gain exceeded metabolism by 5,234–7,876. Failed pursuits cost time and ongoing metabolism. The model currently has **zero additional per-step movement or combat energy charges**; the telemetry reports those as zero rather than inventing costs. Across all carcass origins (predation and natural deaths), about 65–77% of created nutrition was consumed; the remainder decayed or remained at the endpoint. The telemetry separately reports nutrition created by kills, but does not attribute each bite to its carcass of origin. |
| 4 Predator reproduction | Qualified pass | In a favourable 128×96 habitat with 640 prey, two compatible founders produced multiple generations. At year 50, seed 3 had 40 births, 32 mature offspring, generation 4, and 3 living predators; seed 44 had 32 births, 25 mature offspring, generation 4, and 2 living predators. Four other fixed seeds had offspring but went extinct by year 50. This proves replacement is *possible*, not robust. The 64×48 setup failed all six seeds because it did not sustain enough prey for the growing predator family. |
| 5 Controlled predator–prey viability | **Fail** | In the 128×96/640-prey setup at year 100, seed 3 lost predators at tick 42,480 (year 88.5) and seed 44 at tick 34,200 (year 71.25), while 835 and 755 prey remained. Four founders stacked at one prey patch died out at tick 30,840; separating the two pairs into different prey patches delayed extinction only to tick 32,880 (year 68.5), with 761 prey at year 100. Reproduction occurred, but there is no demonstrated century-long predator–prey recovery loop. |
| 6–16 | Not run | Blocked by Gate 5 under the agreed ordering. |

The early hunt failure was traced to abandoning partly eaten carcasses above an energy threshold. Predators now finish nearby carcasses before seeking another hunt; this changed the controlled Gate 2 result from one starvation in six seeds to six survivors. A regression test protects that behavior. The remaining Gate 5 failure is not simply lack of total prey: hundreds remain at predator extinction. Mate encounter/demography and local food competition need independent measurement before another ecological rule change. In particular, seed 44 had only two living females at year 50; its last predator birth was at tick 18,159.

## Reproduce the current checks

```powershell
dotnet test -c Release --no-restore
dotnet run -c Release --no-build -- --benchmark density --ticks 1000
dotnet run -c Release --no-build -- --prey-gate --seed 3 --ticks 48000 --herbivores 24 --width 64 --height 48
dotnet run -c Release --no-build -- --predator-gate --seed 3 --ticks 6000 --herbivores 160 --width 64 --height 48
dotnet run -c Release --no-build -- --predator-reproduction-gate --seed 3 --ticks 24000 --herbivores 640 --predators 2 --width 128 --height 96
dotnet run -c Release --no-build -- --predator-reproduction-gate --seed 3 --ticks 48000 --herbivores 640 --predators 2 --width 128 --height 96
```

The controlled `--predator-reproduction-gate` founder traits and sexes are deliberate. They isolate predator demographic viability from a random founder's niche mismatch; they do not modify ordinary world initialization. A future Gate 5 runner should record year-by-year predator and prey counts, age/sex structure, reproductive opportunities at encounter time, local prey density, and per-predator energy histories. Without that evidence, the population trajectory cannot be called a recovery cycle.
