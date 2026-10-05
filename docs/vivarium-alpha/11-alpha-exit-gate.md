# 11 - Vivarium Alpha Exit Gate

## Purpose

Prove that LivingSim is no longer merely a functioning ecosystem simulator, but a compelling and understandable observer game.

This is the final gate before major ecological breadth work begins.

Do not pass Alpha because the roadmap is “mostly implemented.” Pass only when the experience works as intended.

---

## Gate A - Foundation integrity

Must still pass:

- deterministic benchmark seeds,
- save/reload continuation hashes,
- supported save schema/version handling,
- no player observation feature changes simulation outcomes,
- foundation ecological regression cohort remains within approved behaviour,
- no unresolved Severity 1 simulation/save bug.

Any failure here returns the build to foundation/regression repair.

---

## Gate B - Application flow

Fresh tester can complete without coaching:

`Launch → create seeded world → run → pause → change speed → save → exit → load → continue`

Pass requirements:

- no dead end,
- no ambiguous destructive action,
- seed identity is clear,
- save/load state is clear,
- incompatible/corrupt save handling is safe,
- core flow works in release-like build.

---

## Gate C - Observation usability

Tester must be able to:

- pan and zoom confidently,
- select an animal,
- follow it,
- recover when it dies/disappears,
- inspect its population,
- identify whether that population is rising/falling,
- activate a relevant overlay,
- compare at least two ecological signals.

Target result: high task completion with little or no instruction.

---

## Gate D - History comprehension

Give the tester a world with a known major event and ask:

> What happened, when did it happen, and what evidence can you find around it?

They should be able to use:

- timeline,
- population history,
- graphs,
- world focus,
- lineage/population information.

Pass when players can reconstruct a plausible evidence-based explanation without needing developer/debug tools.

---

## Gate E - Individual attachment

Across observation sessions, test whether players spontaneously:

- keep following an animal,
- favourite/pin an individual or population if supported,
- check descendants,
- notice the death of something they followed,
- ask what happened to a lineage later.

This is partly qualitative.

Alpha does not require every tester to become emotionally attached, but the system must demonstrably support attachment rather than presenting animals as anonymous moving counters.

---

## Gate F - Deep-time watchability

Run structured sessions at multiple scales:

- 10-minute first impression,
- 30-minute observation,
- 60-minute extended session,
- accelerated multi-century session.

Evaluate:

- boredom points,
- confusion points,
- time-control usage,
- whether ecological changes are noticed,
- whether players investigate causes,
- whether high-speed time remains understandable.

Pass when the experience remains comprehensible and gives players reasons to keep watching without intervention mechanics.

---

## Gate G - Visual direction

Must have:

- one formally selected presentation style,
- visual language guide,
- biome/terrain readability rules,
- animal/glyph/sprite readability rules,
- selection/focus rules,
- zoom/LOD rules,
- accessible colour/symbol rules,
- tested dense-scene readability.

No unresolved parallel “maybe this is the art style instead” production path remains.

---

## Gate H - Accessibility

Verify core experience with:

- UI scaling,
- colour-vision checks,
- grayscale/symbol readability,
- reduced-motion option where relevant,
- consistent keyboard/mouse controls,
- readable overlay legends and graphs.

A core ecological distinction must not disappear when colour alone is removed.

---

## Gate I - Performance

On reference target hardware, verify:

- normal observation hits the agreed frame target,
- stress scenes remain above the agreed floor,
- fast-forward delivers agreed simulation throughput,
- history/UI queries do not cause unacceptable stalls,
- memory remains bounded over long sessions,
- no long-run slowdown from event/history accumulation,
- save/load remains inside agreed budgets.

Compare against the frozen `v0.1` foundation to quantify presentation overhead.

---

## Gate J - Stability

Required before Alpha completion:

- zero open Severity 1 bugs,
- zero unwaived Severity 2 bugs on the golden path,
- automated smoke suite green,
- benchmark soak suite green,
- repeated save/load stable,
- long observation session stable,
- release-candidate build tested rather than only developer build.

---

## Gate K - Scope discipline

Confirm that Alpha did **not** become an uncontrolled content-expansion phase.

Before exit:

- catalogue deferred breadth ideas,
- remove/debug-hide incomplete player-facing experiments,
- close or explicitly defer nonessential Alpha features,
- document known debt,
- lock the final Alpha feature list.

---

# Final Alpha evaluation

The build should answer **yes** to all of these:

1. Can a new player create and resume a vivarium without help?
2. Can they comfortably navigate from whole ecosystem to one animal?
3. Can they understand what is happening now?
4. Can they investigate why the world changed?
5. Can they follow a lineage or population through time?
6. Can they notice deep-time ecological change without intervention?
7. Is the experience enjoyable enough to keep watching?
8. Is the visual language readable and distinctive?
9. Does presentation preserve simulation scale and determinism?
10. Is the build stable enough to become the base for more content?

If any answer is no, Alpha remains open.

---

## Passing result

When every gate passes:

1. create an Alpha milestone report,
2. record benchmark/performance/UX results,
3. preserve the passing build/tag,
4. lock the visual direction,
5. unlock the Ecological Breadth Entry Gate,
6. create the next roadmap toward content expansion and Beta.

At this point the project has crossed a critical boundary:

> **LivingSim is a game built around a simulation, not merely a simulation with a viewer.**