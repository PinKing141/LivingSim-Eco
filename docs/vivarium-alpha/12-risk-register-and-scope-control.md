# 12 - Risk Register & Scope Control

## Purpose

Make the main threats to Vivarium Alpha explicit so they can be managed before they become schedule or architecture problems.

The risk register should be reviewed at each milestone gate and whenever a high-impact design decision changes.

---

## Risk scoring

Score each active risk by:

- **Probability:** Low / Medium / High
- **Impact:** Low / Medium / High
- **Detection:** Easy / Moderate / Hard

Prioritise risks that are both likely and expensive to discover late.

---

# Primary Alpha risks

## R1 - Foundation churn during presentation work

**Risk:** UI/art work triggers casual changes to simulation rules, invalidating benchmarks and save determinism.

**Probability:** Medium  
**Impact:** High

### Mitigation

- freeze `v0.1`,
- simulation-version every outcome-changing rule change,
- require benchmark reruns,
- keep presentation read-only by default,
- separate simulation and rendering assemblies/modules where practical.

### Trigger

Golden hashes change after a UI/render-only task.

### Response

Stop feature work and investigate immediately.

---

## R2 - Observer tools become a data-dashboard instead of a game

**Risk:** Too many panels, graphs and metrics overwhelm the calm vivarium experience.

**Probability:** High  
**Impact:** High

### Mitigation

- progressive disclosure,
- default world view stays minimal,
- every panel must answer a player question,
- usability test with fresh users,
- remove metrics that do not change player understanding.

### Trigger

Players spend more time fighting UI than watching the world.

---

## R3 - History/lineage storage grows without bound

**Risk:** old worlds become huge, slow or impossible to save.

**Probability:** Medium  
**Impact:** High

### Mitigation

- tiered history retention,
- downsample aggregates,
- permanent storage only for notable events,
- profile Year 50/250/1,000 saves,
- set explicit bytes/century budget.

### Trigger

Save size or query time grows superlinearly with world age.

---

## R4 - Final art style reduces simulation scale

**Risk:** richer visuals consume enough CPU/GPU/memory that the ecosystem must shrink.

**Probability:** Medium  
**Impact:** High

### Mitigation

- visual prototype performance matrix,
- LOD/zoom rules,
- atlas/batching strategy,
- simulation throughput remains a hard constraint,
- prefer abstraction when density is high.

### Trigger

Presentation cost forces lower animal-count targets.

---

## R5 - Extreme fast-forward becomes unreadable

**Risk:** technically fast simulation produces visual noise and no comprehensible story.

**Probability:** High  
**Impact:** Medium/High

### Mitigation

- separate render cadence from sim cadence,
- reduce animation detail at high speeds,
- emphasise population/environmental changes,
- keep timeline/events updating coherently.

### Trigger

Players pause after fast-forward and cannot tell what happened.

---

## R6 - Event detector creates spam or false narratives

**Risk:** timeline records every fluctuation or implies unsupported causation.

**Probability:** High  
**Impact:** Medium/High

### Mitigation

- hysteresis,
- significance thresholds,
- minimum durations,
- evidence-linked wording,
- event deduplication,
- test known benchmark histories.

### Trigger

Timeline becomes noisy or players believe claims the simulation cannot support.

---

## R7 - Visual language depends too heavily on colour

**Risk:** species/biomes become indistinguishable for some players or under overlays.

**Probability:** Medium  
**Impact:** Medium

### Mitigation

- unique symbols/shapes,
- grayscale tests,
- colour-vision simulation,
- high-contrast selected states,
- legends and labels.

---

## R8 - Feature creep before Alpha proof

**Risk:** disease, fire, fish, new species, speciation and other attractive systems enter before the observer experience is solved.

**Probability:** High  
**Impact:** High

### Mitigation

- breadth lock,
- separate deferred backlog,
- require Alpha Exit Gate,
- admission test for any exception,
- capped milestone scope.

### Trigger

A new ecological system is proposed without being required by an Alpha acceptance criterion.

---

## R9 - Solo-development process overload

**Risk:** “AAA process” becomes more work than the game.

**Probability:** Medium  
**Impact:** Medium

### Mitigation

Only keep process artifacts that answer a real production question.

Keep:

- phase gates,
- tests,
- benchmarks,
- risk register,
- definition of done,
- short milestone review.

Avoid:

- unnecessary recurring meetings,
- duplicate trackers,
- documentation with no owner/use,
- ceremony for its own sake.

---

## R10 - Players do not care about individual animals

**Risk:** simulation is informative but emotionally sterile.

**Probability:** Medium  
**Impact:** High

### Mitigation

- follow mode,
- ancestry/descendants,
- pin/favourite,
- life/death summaries,
- readable current behaviour,
- long-session observation tests.

### Trigger

Testers never return to selected animals or lineages.

---

## R11 - Players cannot explain ecological change

**Risk:** interesting events happen, but observation tools fail to reveal them.

**Probability:** Medium  
**Impact:** High

### Mitigation

- graph ↔ timeline ↔ world cross-linking,
- clear population trends,
- resource/climate overlays,
- evidence-based events,
- usability tasks specifically asking “why?”.

### Trigger

Testers notice a crash but cannot investigate it using player tools.

---

# Scope-control rules

## Rule 1 - One milestone question

Every phase has one primary question.

Do not add work that does not help answer it.

## Rule 2 - New feature requires a cut or explicit budget

If a significant unplanned feature enters a milestone, identify what schedule/scope budget it consumes.

There is no “free” feature.

## Rule 3 - Prototype before pipeline

Do not build production architecture for an uncertain visual/UX idea before a cheap prototype proves it.

## Rule 4 - Fix root causes at the lowest layer

If a problem is simulation correctness, do not hide it with UI.

If a problem is comprehension, do not add ecology to solve it.

If a problem is presentation, do not retune the ecosystem.

## Rule 5 - Phase gate means stop

When the phase acceptance criteria pass, move on.

Do not endlessly polish one phase while downstream risk remains untested.

---

# Milestone risk review template

At each phase exit, record:

- top five active risks,
- whether probability changed,
- whether impact changed,
- newly discovered risks,
- mitigation owner/action,
- risk accepted, reduced, transferred, deferred or closed.

---

## Final principle

The roadmap should converge.

Every production phase should reduce uncertainty until the project reaches a stable, testable observer experience.

If the roadmap keeps expanding faster than risks are being retired, scope control has failed.