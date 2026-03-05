using System;
using System.Collections.Generic;
using LivingSim.World;
using LivingSim.Core;
using LivingSim.Animals;

namespace LivingSim.Environment
{
    public class AnimalManager
    {
        private readonly List<Animal> _animals = new List<Animal>();
        private readonly int _width;
        private readonly int _height;
        private readonly Random _random;

        // Plague Settings
        private const int PlagueCheckInterval = 100;
        private const float PlagueThresholdRatio = 0.20f;
        private const float PlagueChance = 0.05f;
        private const float PlagueMortalityRate = 0.4f;

        public AnimalManager(int width, int height, Random random)
        {
            _width = width;
            _height = height;
            _random = random;
        }

        public void SpawnAnimal(AnimalType type, int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return;

            Species species = type switch
            {
                AnimalType.Herbivore => _random.NextDouble() < 0.5 ? Species.Deer : Species.Rabbit,
                AnimalType.Omnivore => _random.NextDouble() < 0.5 ? Species.Bear : Species.Boar,
                AnimalType.Carnivore => _random.NextDouble() < 0.5 ? Species.Wolf : Species.Fox,
                _ => throw new ArgumentOutOfRangeException(nameof(type), "Unknown AnimalType for species assignment.")
            };

            _animals.Add(new Animal(species, x, y, _random));
        }

        public void Tick(Grid grid, long currentTick, bool isNight, Season currentSeason)
        {
            var animalsThisTick = new List<Animal>(_animals);
            EnsureGridRegistrations(grid, animalsThisTick);

            for (int i = 0; i < animalsThisTick.Count; i++)
            {
                var animal = animalsThisTick[i];
                if (animal.IsAlive)
                {
                    animal.Move(grid, animalsThisTick, currentTick, isNight, currentSeason);
                }
            }

            HandleSocialDynamics(animalsThisTick);

            for (int i = 0; i < animalsThisTick.Count; i++)
            {
                var animal = animalsThisTick[i];
                if (!animal.IsAlive || animal.Type == AnimalType.Herbivore) continue;
                var cell = grid.GetCell(animal.X, animal.Y);
                if (cell != null && cell.Residents.Count > 1)
                {
                    animal.Hunt(cell.Residents, grid);
                }
            }

            for (int i = 0; i < animalsThisTick.Count; i++)
            {
                var animal = animalsThisTick[i];
                if (!animal.IsAlive) continue;
                var cell = grid.GetCell(animal.X, animal.Y);
                if (cell != null)
                {
                    animal.EatAndAge(cell, cell.Residents, grid);
                }
            }

            var newborns = new List<Animal>();
            for (int i = 0; i < animalsThisTick.Count; i++)
            {
                var animal = animalsThisTick[i];
                int nearbySameSpecies = CountNearbySameSpecies(grid, animal);

                if (!animal.IsCrowded(nearbySameSpecies) && animal.IsReadyToReproduce() && _random.NextDouble() < (Animal.BaseReproductionChance * animal.Fertility))
                {
                    newborns.AddRange(animal.CreateOffspring(_random));
                }
            }

            for (int i = 0; i < newborns.Count; i++)
            {
                var newborn = newborns[i];
                _animals.Add(newborn);
                grid.RegisterAnimal(newborn);
            }

            for (int i = 0; i < animalsThisTick.Count; i++)
            {
                var animal = animalsThisTick[i];
                if (!animal.IsAlive)
                {
                    animal.DecayCarcass();
                }
            }

            for (int i = _animals.Count - 1; i >= 0; i--)
            {
                var animal = _animals[i];
                if (!animal.IsAlive && animal.CarcassFoodValue <= 0)
                {
                    grid.UnregisterAnimal(animal);
                    _animals.RemoveAt(i);
                }
            }

            if (currentTick % PlagueCheckInterval == 0)
            {
                HandlePlagueEvents();
            }
        }

        public IReadOnlyList<Animal> GetAnimals() => _animals;

        private void EnsureGridRegistrations(Grid grid, List<Animal> animals)
        {
            for (int i = 0; i < animals.Count; i++)
            {
                var a = animals[i];
                var cell = grid.GetCell(a.X, a.Y);
                if (cell != null && !cell.Residents.Contains(a))
                {
                    grid.RegisterAnimal(a);
                }
            }
        }

        private int CountNearbySameSpecies(Grid grid, Animal animal)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    var neighborsCell = grid.GetCell(animal.X + dx, animal.Y + dy);
                    if (neighborsCell == null) continue;
                    var residents = neighborsCell.Residents;
                    for (int i = 0; i < residents.Count; i++)
                    {
                        var neighbor = residents[i];
                        if (neighbor.IsAlive && neighbor.Species == animal.Species)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private void HandleSocialDynamics(List<Animal> animalsThisTick)
        {
            var groups = new Dictionary<Guid, List<Animal>>();
            for (int i = 0; i < animalsThisTick.Count; i++)
            {
                var animal = animalsThisTick[i];
                if (!animal.IsAlive) continue;
                if (!groups.TryGetValue(animal.GroupId, out var list))
                {
                    list = new List<Animal>();
                    groups[animal.GroupId] = list;
                }
                list.Add(animal);
            }

            foreach (var kvp in groups)
            {
                var group = kvp.Value;
                if (group.Count <= Animal.MaxGroupSize) continue;

                Animal? expellee = null;
                for (int i = 0; i < group.Count; i++)
                {
                    var candidate = group[i];
                    if (candidate.Age >= Animal.MinReproductionAge && !candidate.IsHibernating)
                    {
                        if (expellee == null || candidate.Age < expellee.Age)
                        {
                            expellee = candidate;
                        }
                    }
                }

                if (expellee == null) continue;

                Guid closestGroupId = Guid.Empty;
                Animal? closestRepresentative = null;
                double closestDistance = double.MaxValue;

                foreach (var other in groups)
                {
                    if (other.Key == expellee.GroupId || other.Value.Count >= Animal.MaxGroupSize || other.Value.Count == 0) continue;
                    if (other.Value[0].Species != expellee.Species) continue;

                    double sumX = 0;
                    double sumY = 0;
                    for (int i = 0; i < other.Value.Count; i++)
                    {
                        sumX += other.Value[i].X;
                        sumY += other.Value[i].Y;
                    }

                    double centerX = sumX / other.Value.Count;
                    double centerY = sumY / other.Value.Count;
                    double distSq = Math.Pow(expellee.X - centerX, 2) + Math.Pow(expellee.Y - centerY, 2);
                    if (distSq < closestDistance)
                    {
                        closestDistance = distSq;
                        closestGroupId = other.Key;
                        closestRepresentative = other.Value[0];
                    }
                }

                if (closestRepresentative != null)
                {
                    expellee.JoinGroup(closestGroupId);
                    expellee.SetDen(closestRepresentative.DenX, closestRepresentative.DenY);
                }
                else
                {
                    expellee.LeaveGroup();
                }
            }
        }

        private void HandlePlagueEvents()
        {
            var speciesCounts = new Dictionary<Species, int>();
            for (int i = 0; i < _animals.Count; i++)
            {
                var animal = _animals[i];
                if (!animal.IsAlive) continue;
                speciesCounts.TryGetValue(animal.Species, out int count);
                speciesCounts[animal.Species] = count + 1;
            }

            int mapArea = _width * _height;
            foreach (var kvp in speciesCounts)
            {
                if (kvp.Value > mapArea * PlagueThresholdRatio && _random.NextDouble() < PlagueChance)
                {
                    TriggerPlague(kvp.Key);
                }
            }
        }

        private void TriggerPlague(Species species)
        {
            var targets = new List<Animal>();
            for (int i = 0; i < _animals.Count; i++)
            {
                var animal = _animals[i];
                if (animal.IsAlive && animal.Species == species)
                {
                    targets.Add(animal);
                }
            }

            int killCount = (int)(targets.Count * PlagueMortalityRate);
            targets.Sort((a, b) =>
            {
                int healthCompare = a.Health.CompareTo(b.Health);
                if (healthCompare != 0) return healthCompare;
                return b.Age.CompareTo(a.Age);
            });

            for (int i = 0; i < killCount && i < targets.Count; i++)
            {
                targets[i].TakeDamage(targets[i].MaxHealth * 2, null);
            }
        }
    }
}
