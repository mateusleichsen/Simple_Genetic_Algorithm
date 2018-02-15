using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Genetic
{
    public class PopulationCrafter
    {
        private int bodyLength;
        private double limitMultiplicatePopulation;
        private double mutationDeviant;
        private Random random;

        public Population Individuals { get; set; }

        public PopulationCrafter(int bodyLength, double limitMultiplicatePopulation = 2, double mutationDeviant = 0.2)
        {
            this.bodyLength = bodyLength;
            this.limitMultiplicatePopulation = limitMultiplicatePopulation;
            this.mutationDeviant = mutationDeviant;
            this.random = RandomHelper.GenerateByDateTimeTicks();
        }
        
        public void ProcreateByOldPopulation()
        {
            Individuals = MatchPopulation(Individuals);
            MutatePopulation(Individuals);
        }

        public void ProcreateBySelectedPopulation(Population oldIndividuals, int populationLimit)
        {
            Individuals = MatchPopulation(oldIndividuals, populationLimit);
            MutatePopulation(Individuals);
        }

        private void MutatePopulation(Population newPopulation)
        {
            for (int i = 0; i < newPopulation.Count * mutationDeviant; i++)
            {
                var index = random.Next(0, newPopulation.Count);
                newPopulation[index] = Mutate(newPopulation[index]);
            }
        }

        private Population MatchPopulation(Population oldPopulation)
        {
            var newPopulation = new Population();
            do
            {
                var firstIndividualIndex = random.Next(0, oldPopulation.Count);
                var secondIndividualIndex = random.Next(0, oldPopulation.Count);

                var firstIndividual = oldPopulation[firstIndividualIndex];
                var secondIndividual = oldPopulation[secondIndividualIndex];

                var nextGeneration = Crossover(firstIndividual, secondIndividual);
                newPopulation.AddRange(nextGeneration);

            } while (newPopulation.Count < oldPopulation.Count * limitMultiplicatePopulation);

            return newPopulation;
        }

        private Population MatchPopulation(Population oldIndividuals, int populationLimit)
        {
            var newPopulation = new Population();
            do
            {
                var firstIndividual = oldIndividuals[random.Next(0, oldIndividuals.Count - 1)];
                var secondIndividual = oldIndividuals[random.Next(0, oldIndividuals.Count - 1)];

                var nextGeneration = Crossover(firstIndividual, secondIndividual);

                newPopulation.AddRange(nextGeneration);
            } while (newPopulation.Count < populationLimit);

            return newPopulation;
        }

        public void EliminateWeaks(int takeQuantity)
        {
            Individuals.Sort(new IndividualComparer());
            Individuals = Individuals.OrderByDescending(x => x.Fitness).Take(takeQuantity).ToPopulation();
        }

        Population Crossover(Individual firstIndividual, Individual secondIndividual)
        {
            var bodySeparation = firstIndividual.Identity.Length / 2;

            var oneIdentityFirstPart = firstIndividual.Identity.Substring(0, bodySeparation);
            var twoIdentityFirstPart = secondIndividual.Identity.Substring(0, bodySeparation);
            var oneIdentityLastPart = firstIndividual.Identity.Substring(bodySeparation);
            var twoIdentityLastPart = secondIndividual.Identity.Substring(bodySeparation);

            var individualChildOne = new Individual(oneIdentityFirstPart + twoIdentityLastPart);
            var individualChildTwo = new Individual(twoIdentityFirstPart + oneIdentityLastPart);

            return new Population() { individualChildOne, individualChildTwo };
        }

        Individual Mutate(Individual individual) => Mutate(individual.Identity);

        Individual Mutate(string identity)
        {
            var individualMutated = new StringBuilder(identity);
            
            for (int i = 0; i < this.bodyLength * this.mutationDeviant; i++)
            {
                var geneMutated = random.Next(0, this.bodyLength);
                var gene = Convert.ToChar(random.Next(32, 123));
                individualMutated[geneMutated] = gene;
            }

            return new Individual(individualMutated.ToString());
        }

        public Population CreateFirstPopulation(int populationLimit)
        {
            var population = new Population();

            for (int i = 0; i < populationLimit; i++)
            {
                var individual = this.CreateIndividual();
                population.Add(individual);
            }

            return population;
        }

        public Individual CreateIndividual()
        {
            var identity = string.Empty;

            for (int i = 0; i < this.bodyLength; i++)
            {
                var gene = Convert.ToChar(random.Next(32, 122));
                identity += gene.ToString();
            }

            return new Individual(identity);
        }
    }
}
