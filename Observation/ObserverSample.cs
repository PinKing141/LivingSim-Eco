namespace LivingSim.Observation;

public readonly record struct ObserverSample(long Tick, int Herbivores, int Predators, double PlantEaterSpeed);
