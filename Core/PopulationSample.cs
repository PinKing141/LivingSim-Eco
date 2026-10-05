namespace LivingSim.Core;

public readonly record struct PopulationSample(long Tick, int Herbivores, int Predators, long PlantBiomass, double AverageHabitatPressure);
