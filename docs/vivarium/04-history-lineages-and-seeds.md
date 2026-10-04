# 04 - History, Lineages & Seeds

## Why history matters

The simulation should preserve enough history for the player to understand important changes without attempting to store every event forever.

The game becomes more compelling when the player can answer:

- What changed?
- When did it change?
- Why did it change?
- Which animals or populations were involved?
- What survives from that event today?

## Timeline events

The game should be able to detect and record notable events such as:

- a species or population establishing itself in a new region,
- major population collapse,
- major recovery,
- extinction of a species,
- extinction of a notable lineage,
- severe drought,
- prolonged abundance,
- large migration or range expansion,
- meaningful trait milestones,
- sustained population divergence,
- record population highs or lows.

The event system should prioritise **interesting changes** rather than flooding the player with routine simulation messages.

## Lineages

An inspected animal should be traceable through descendants.

Over long simulations, the player may discover that a large fraction of a modern population descends from an individual or early group they once followed.

This creates emotional attachment without turning the game into character management.

Potential lineage tools include:

- parent links,
- offspring links,
- generation count,
- descendant count,
- notable descendants,
- percentage of a current population descended from an ancestor,
- lineage extinction date,
- trait change across descendants.

The exact amount of genealogical data retained must remain scalable.

## Population identity

A species should not always be treated as one homogeneous global population.

If groups become geographically isolated, the simulation may eventually track local populations such as:

- Northern Wolves,
- Western River Deer,
- Island Rabbits.

These names do not need to represent formal biological speciation. They can simply make long-term divergence easier for the player to understand.

## Seed culture

Seeds are central to the concept because the interesting object is not only the generated map; it is the natural history produced from the starting conditions.

A memorable seed might become known for:

- unusual biome geography,
- immediate predator extinction,
- a tiny refuge supporting the last surviving population,
- repeated ecological collapses,
- unusually successful omnivores,
- dramatic climate pressure,
- unexpected evolutionary trajectories.

Players should be able to share seeds and compare what happened.

## Deterministic history

The long-term goal is that identical simulation versions, settings, and seeds produce identical histories.

This creates several benefits:

- reproducible bug reports,
- benchmark seeds,
- deterministic tests,
- community seed sharing,
- repeatable ecological experiments.

Whether a seed should reproduce the exact same history across **different game versions** remains intentionally unresolved. A practical approach is to guarantee determinism only within the same simulation version and store that version with saves and shared seed metadata.

## History storage principle

Store detailed recent information and important long-term summaries rather than every low-value event forever.

The history system should be designed around questions the player may actually ask, not archival completeness.
