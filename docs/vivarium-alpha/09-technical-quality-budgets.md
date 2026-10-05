# 09 - Technical Quality Budgets

## Purpose

Give Vivarium Alpha measurable technical limits before polish and content growth consume the available performance, memory and storage headroom.

Budgets are guardrails. They should be calibrated on the target hardware after `v0.1` is frozen, then treated as regression targets.

---

## Baseline rule

Do not invent a fresh baseline after every optimisation or feature.

At Foundation Freeze record:

- reference machine,
- OS/runtime,
- build configuration,
- benchmark seed,
- entity count,
- ticks simulated,
- elapsed time,
- peak memory,
- save/load times.

The currently observed pre-freeze figure of roughly 1,130 animals for 1,000 ticks in about 1.7–1.9 seconds is useful context, but the official Alpha budget begins from the passing `v0.1` capture.

---

## Frame-time budget

If the presentation targets 60 FPS at normal observation speed:

- total frame budget: **16.67 ms**,
- simulation contribution must not monopolise the frame,
- rendering/UI must have explicit sub-budgets,
- expensive graph/history computation should be amortised or asynchronous where safe.

If the final presentation target changes, recalculate rather than silently ignoring the budget.

### Stress floor

Define a minimum acceptable frame rate for intentionally difficult scenes such as:

- maximum visible population,
- dense biome,
- selection + overlay + graph open,
- camera movement,
- seasonal effects.

A stress floor is not permission for routine play to run at that rate.

---

## Simulation throughput budget

Maintain separate targets for:

### Normal presentation mode

Simulation keeps pace with the selected player time rate while rendering smoothly.

### Fast-forward mode

Presentation updates may be reduced so simulation throughput dominates.

### Headless/extreme mode

Maximum practical ticks/second with no unnecessary rendering work.

Track throughput across at least:

- 1,000 entities,
- 2,000 entities,
- 5,000 entities,
- worst-case density scenario,
- long-history world.

Watch scaling shape, not just one number. A sudden curve toward O(N²) behaviour is a blocker.

---

## Input and camera latency

Player input should feel immediate even while the simulation is busy.

Track:

- click-to-selection response,
- pause response,
- speed-change response,
- camera start/stop response,
- panel open/close latency.

If expensive simulation work can block the UI thread, redesign the scheduling rather than masking it with animation.

---

## Memory budget

Track separately:

- live simulation state,
- spatial structures,
- renderer assets,
- UI/history caches,
- save buffers,
- temporary allocations.

Required captures:

- fresh world,
- Year 50,
- Year 250,
- Year 1,000,
- high population,
- long session with repeated graph/timeline use.

Memory must not grow indefinitely due to observation history or UI caches.

---

## Allocation budget

Hot simulation paths should remain allocation-free or near-zero as established by the foundation architecture.

Presentation may allocate more freely, but recurring per-frame/per-tick allocations must be profiled.

Track:

- bytes allocated/frame,
- bytes allocated/simulation tick,
- garbage-collection frequency,
- largest transient spikes.

GC spikes that interrupt observation or fast-forward are performance bugs.

---

## Save budget

Measure:

- save size,
- save time,
- load time,
- temporary memory during serialisation,
- growth per simulated century.

History/lineage must have explicit retention/downsampling rules so a 1,000-year world does not become impractical to save.

Recommended policy:

- player should receive progress feedback for operations that exceed the “instant” threshold,
- save/load should never freeze without feedback,
- autosave should avoid interrupting critical interaction where practical.

---

## History computation budget

Potentially expensive queries include:

- descendant counts,
- ancestry expansion,
- population range history,
- trait distributions,
- long graph windows,
- event correlation views.

Classify each query as:

- real-time,
- cached,
- incremental,
- background/deferred.

Do not perform unbounded ancestry or historical scans every frame.

---

## Rendering budgets

After Phase 4 direction lock, define:

- maximum practical visible glyphs/sprites,
- atlas/texture memory,
- draw-call/batch expectations,
- overlay composition cost,
- zoom-level LOD policy,
- animation update budget.

The art style must fit the simulation, not force the simulation to shrink around the art style.

---

## Regression thresholds

For benchmark tests, define an allowed variance band.

Suggested process:

- capture several runs,
- use median rather than one lucky run,
- distinguish measurement noise from real regression,
- investigate sustained regressions above the agreed percentage,
- require written justification for intentionally spending performance headroom.

Performance is a resource budget, like memory or content scope.

---

## Profiling cadence

Profile:

- at the start of a phase,
- after integrating the riskiest feature,
- before phase exit,
- after visual direction lock,
- during feature lock before Alpha Exit.

Do not postpone all profiling until the end.

---

## Technical gate

No Alpha phase passes if it:

- breaks determinism unintentionally,
- causes save corruption,
- exceeds agreed memory/storage growth,
- creates unacceptable frame-time spikes,
- materially degrades benchmark throughput without approved trade-off,
- introduces long-run slowdown that worsens with world age.

Quality budgets are part of feature completion, not optional optimisation work.