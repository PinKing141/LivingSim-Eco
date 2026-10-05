# 04 - Phase 3: History & Lineage

## Goal

Allow the player to understand not only what exists now, but how the world arrived there.

This phase turns raw simulation state into **natural history**.

---

## Core player questions

The system should help answer:

- Where did this population come from?
- Why did it decline?
- What happened during this drought?
- Which animals founded this lineage?
- Are these two regional populations related?
- When did this species peak?
- What caused this extinction?
- Did this trait change before or after the environmental pressure?

---

## Lineage system

### Individual ancestry

For a selected animal, expose:

- parents,
- grandparents where available,
- offspring,
- descendant count,
- generation,
- founder/reference ancestors where meaningful.

The UI should not attempt to render an unlimited family tree all at once.

Use expandable generations, focused branches and summary counts.

### Descendant tracing

Support questions such as:

> How much of the present wolf population descends from this animal?

Potential outputs:

- living descendants,
- total known descendants,
- descendant share of current population,
- regions where descendants now live,
- generations elapsed.

These calculations may be cached or performed asynchronously if expensive.

---

## Population history

Each tracked population/species should expose:

- origin/start date,
- population curve,
- historic high/low,
- geographic range over time,
- major birth/death eras,
- trait changes,
- bottlenecks,
- recoveries,
- migrations,
- local/regional extinction.

Population identity rules must be deterministic and documented.

---

## Event detection

Create a rule-based event detector for noteworthy natural history.

Candidate event classes:

- population boom,
- population crash,
- recovery from low population,
- local extinction,
- global extinction,
- colonisation of a new region,
- major migration,
- prolonged drought/harsh climate era,
- resource collapse,
- resource recovery,
- unusual mortality event,
- new population high/low,
- significant trait milestone,
- geographic divergence milestone,
- notable lineage expansion.

### Event quality rules

An event should be recorded only when it is:

- statistically or ecologically significant enough to matter,
- understandable in player language,
- linked to real simulation evidence,
- non-duplicative,
- stable enough that tiny one-tick fluctuations do not spam the timeline.

Use thresholds, hysteresis and minimum-duration rules.

---

## Timeline

The world timeline should support:

- chronological browsing,
- filters by species/population/event type,
- jump to date,
- jump to map location where relevant,
- jump to related graph range,
- inspect before/after state,
- show links between related events where confidence is sufficient.

The timeline should report evidence without pretending to know causes the simulation cannot prove.

For example:

Good:

> Rabbit population fell 62% during a 14-year drought while regional biomass reached a historic low.

Avoid overclaiming:

> The drought definitely caused the rabbit collapse.

unless causation is directly encoded and supported.

---

## Historical storage strategy

Do not preserve every simulation tick forever.

Use tiered retention:

### Full current state

Needed for active simulation.

### High-resolution recent history

Useful for current graphs and recent events.

### Downsampled long-term history

Population, climate and trait aggregates at coarser intervals.

### Permanent notable events

Small structured records for important history.

### Selective lineage data

Preserve enough ancestry to support the promised player experience without retaining every dead entity's entire runtime state.

---

## Data budget

Track and profile:

- bytes per living animal,
- bytes per dead historical record,
- lineage growth per simulated century,
- event count per century,
- graph/history aggregate growth,
- save-file growth at Year 50/250/1,000.

History is not allowed to make old worlds progressively unusable.

---

## Cross-linking requirement

The strongest experience comes from connecting systems.

Target flows:

`Animal → Parent → Population → Population graph → Event → World location`

and

`Timeline event → Species → Trait graph → Regional population → Descendants`

Avoid isolated screens that require the player to mentally reconstruct connections.

---

## QA scenarios

Test:

- animal with no known parents,
- animal with many descendants,
- extinct lineage,
- population split across regions,
- local extinction but global survival,
- species global extinction,
- very old world,
- timeline with many events,
- save/reload with historical data,
- version migration where supported.

---

## Phase exit gate

Phase 3 passes when:

- ancestry/descendant exploration works at useful scale,
- population histories remain understandable over centuries,
- event detection produces meaningful events without spam,
- timeline entries link back to supporting data/world locations,
- long-run history remains inside storage/performance budgets,
- save/reload preserves history correctly,
- testers can answer at least one “how did this happen?” question using only in-game tools.

Only then begin Phase 4.