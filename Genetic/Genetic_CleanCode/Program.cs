using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetic
{
    class Program
    {
        static void Main(string[] args)
        {
            var objective = "der hund schlaft";
            TestFixePopulation(objective, 1000, 1000);
            Console.ReadKey();
            TestAutoGeneratePopulation(objective, 2000, 1000);
            Console.ReadKey();
        }

        private static void TestAutoGeneratePopulation(string objective, int populationLimit, int generationLimit)
        {
            var population = new PopulationCrafter(objective.Length, 3, 0.15);
            population.Individuals = new Population() { population.CreateIndividual(), population.CreateIndividual() };

            double bestFitness = 0;
            var countGenerations = 0;
            do
            {
                countGenerations++;
                population.ProcreateByOldPopulation();

                PopulationFitnessHelper.CalculateFitness(objective, population.Individuals);
                population.Individuals.Sort(new IndividualComparer());

                var bestIndividual = population.Individuals.Last();
                PrintIndividualProfile(bestIndividual);
                bestFitness = bestIndividual.Fitness;
                
                while (population.Individuals.Count > populationLimit)
                {
                    population.EliminateWeaks(800);
                }
            } while (bestFitness < 100 && countGenerations < generationLimit);

            Console.WriteLine($"generations: {countGenerations}");
        }

        private static void TestFixePopulation(string objective, int populationLimit, int generationLimit)
        {
            var population = new PopulationCrafter(objective.Length);
            var individuals = population.CreateFirstPopulation(populationLimit);

            PopulationFitnessHelper.CalculateFitness(objective, individuals);
            individuals.Sort(new IndividualComparer());
            var bestIndividuals = individuals.OrderByDescending(x => x.Fitness).Take(50).ToPopulation();

            double bestFitness = 0;
            var countGenerations = 0;
            do
            {
                countGenerations++;
                population.ProcreateBySelectedPopulation(bestIndividuals, populationLimit);
                PopulationFitnessHelper.CalculateFitness(objective, population.Individuals);
                population.Individuals.Sort(new IndividualComparer());
                bestIndividuals = population.Individuals.OrderByDescending(x => x.Fitness).Take(50).ToPopulation();

                var firstIndividual = bestIndividuals.FirstOrDefault();
                PrintIndividualProfile(firstIndividual);
                bestFitness = firstIndividual.Fitness;
            } while (bestFitness < 100 && countGenerations < generationLimit);

            Console.WriteLine($"generations: {countGenerations}");
        }

        static void PrintIndividualProfile(Individual firstIndividual) 
            => Console.WriteLine($"{firstIndividual.Identity} - {firstIndividual.Fitness}");
    }
}
