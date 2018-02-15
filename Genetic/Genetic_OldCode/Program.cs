using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genetic
{
    class Program
    {
        const string objective = "o cachorro dormiu sobre a cama do gato";
        static void Main(string[] args)
        {
            TestFixePopulation();
            Console.ReadKey();
            TestAutoGeneratePopulation();
            Console.ReadKey();
        }

        private static void TestAutoGeneratePopulation()
        {
            var popu = new List<string>() { Population.CreateIndividual(objective.Length), Population.CreateIndividual(objective.Length) };

            double bestApti = 0;
            var countGen = 0;
            do
            {
                countGen++;
                var newGeneration = Population.Procreate3(popu, objective.Length, 2, 0.6);
                popu = newGeneration;

                var result = GetApti(newGeneration);
                result.Sort((pair1, pair2) => pair2.Item2.CompareTo(pair1.Item2));

                var firstItem = result.FirstOrDefault();
                Console.WriteLine(firstItem.Item1 + " - " + firstItem.Item2);
                bestApti = firstItem.Item2;
                while (popu.Count > 2000)
                {
                    popu = Population.EliminateWeaks(result, 900);
                }
            } while (bestApti < 100 && countGen < 10000);

            Console.WriteLine($"generations: {countGen}");
        }

        private static void TestFixePopulation()
        {
            var popu = Population.CreateFirstPopulation(objective.Length);
            var result = GetApti(popu);
            result.Sort((pair1, pair2) => pair2.Item2.CompareTo(pair1.Item2));
            var top50 = result.Take(50).Select(x => x.Item1).ToList();

            double bestApti = 0;
            var countGen = 0;
            do
            {
                countGen++;
                var newGeneration = Population.Procreate(top50, objective.Length);
                result = GetApti(newGeneration);
                result.Sort((pair1, pair2) => pair2.Item2.CompareTo(pair1.Item2));
                top50 = result.Take(50).Select(x => x.Item1).ToList();

                var firstItem = result.FirstOrDefault();
                Console.WriteLine(firstItem.Item1 + " - " + firstItem.Item2);
                bestApti = firstItem.Item2;
            } while (bestApti < 100 && countGen < 5000);

            Console.WriteLine($"generations: {countGen}");
        }

        static List<Tuple<string, double>> GetApti(List<string> popu)
        {
            return CalculateFitness(popu);
        }
        
        private static List<Tuple<string, double>> CalculateFitness(List<string> population)
        {
            var newList = new List<Tuple<string, double>>();
            foreach (var individual in population)
            {
                var count = 0;
                for (int i = 0; i < individual.Length; i++)
                {
                    if (individual[i] == objective[i])
                        count++;
                }
                
                double fitness = (double)count / (double)objective.Length;
                fitness = Math.Pow(2, fitness);
                fitness = fitness * 100 / 2;
                newList.Add(new Tuple<string, double>(individual, fitness));
            }

            return newList;
        }

        public static Random GenerateRandom()
        {
            var a = (int)DateTime.Now.Ticks;
            return new Random(a);
        }

    }

    public class Population
    {
        public static List<string> Procreate2(List<string> oldPopulation, int objectiveLength)
        {
            var r = Program.GenerateRandom();
            var newPopulation = new List<string>();
            var maxPopulation = oldPopulation.Count;
            do
            {

                var firstInd = r.Next(0, maxPopulation);
                var secondInd = r.Next(0, maxPopulation);
                while (firstInd == secondInd)
                {
                    secondInd = r.Next(0, maxPopulation);
                }

                var one = oldPopulation[firstInd];
                var two = oldPopulation[secondInd];

                var nextGeneration = Crossover(one, two);
                newPopulation.AddRange(nextGeneration);

            } while (newPopulation.Count < oldPopulation.Count);

            for (int i = 0; i < newPopulation.Count / 2; i++)
            {
                var index = r.Next(0, newPopulation.Count);
                newPopulation[index] = Mutate(newPopulation[index], objectiveLength, 0.1);
            }

            newPopulation.AddRange(oldPopulation);

            return newPopulation;
        }

        public static List<Individual> EliminateWeaks(List<Individual> population, int limitMax)
        {
            population.Sort((individual1, individual2) => individual2.Fitness.CompareTo(individual1.Fitness));
            return population.Take(limitMax).ToList();
        }

        public static List<string> Procreate3(List<string> oldPopulation, int objectiveLength, double multiPopulation, double mutation)
        {
            var r = Program.GenerateRandom();
            var newPopulation = new List<string>();
            var maxPopulation = oldPopulation.Count;
            do
            {
                var firstInd = r.Next(0, maxPopulation);
                var secondInd = r.Next(0, maxPopulation);
                while (firstInd == secondInd)
                {
                    secondInd = r.Next(0, maxPopulation);
                }

                var one = oldPopulation[firstInd];
                var two = oldPopulation[secondInd];

                var nextGeneration = Crossover(one, two);
                newPopulation.AddRange(nextGeneration);

            } while (newPopulation.Count < oldPopulation.Count * multiPopulation);

            for (int i = 0; i < newPopulation.Count * mutation; i++)
            {
                var index = r.Next(0, newPopulation.Count);
                newPopulation[index] = Mutate(newPopulation[index], objectiveLength, 0.1);
            }

            return newPopulation;
        }

        public static List<string> Procreate(List<string> oldPopulation, int objectiveLength)
        {
            var r = Program.GenerateRandom();
            var newPopulation = new List<string>();
            var maxPopulation = oldPopulation.Count - 1;

            do
            {
                var one = oldPopulation[r.Next(0, maxPopulation)];
                var two = oldPopulation[r.Next(0, maxPopulation)];

                var nextGeneration = Crossover(one, two);

                newPopulation.AddRange(nextGeneration);
            } while (newPopulation.Count < 500);

            for (int i = 0; i < 250; i++)
            {
                var index = r.Next(0, 499);
                newPopulation[index] = Mutate(newPopulation[index], objectiveLength, 0.5);
            }

            return newPopulation;
        }

        static string[] Crossover(string one, string two)
        {
            var halfIndex = one.Length / 2;
            var result = string.Empty;
            var oneSplit1 = one.Substring(0, halfIndex);
            var twoSplit1 = two.Substring(0, halfIndex);
            var oneSplit2 = one.Substring(halfIndex);
            var twoSplit2 = two.Substring(halfIndex);

            return new string[] { oneSplit1 + twoSplit2, twoSplit1 + oneSplit2 };
        }

        static string Mutate(string one, int objectiveLength, double mutation)
        {
            StringBuilder sb = new StringBuilder(one);

            int a = (int)DateTime.Now.Ticks;
            var r = Program.GenerateRandom();

            for (int i = 0; i < objectiveLength * mutation; i++)
            {
                var genMut = r.Next(0, objectiveLength);
                var c = Convert.ToChar(r.Next(32, 123));
                sb[genMut] = c;
            }

            return sb.ToString();
        }

        public static List<string> CreateFirstPopulation(int objectiveLength)
        {
            var population = new List<string>();

            for (int i = 0; i < 500; i++)
            {
                var one = CreateIndividual(objectiveLength);
                population.Add(one);
            }

            return population;
        }

        public static string CreateIndividual(int objectiveLength)
        {
            var result = string.Empty;

            for (int i = 0; i < objectiveLength; i++)
            {
                var c = Convert.ToChar(Program.GenerateRandom().Next(32, 122));
                result += c.ToString();
            }

            return result;
        }

    }
}
