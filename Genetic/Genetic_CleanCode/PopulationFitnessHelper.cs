using System;
using System.Collections.Generic;
using System.Text;

namespace Genetic
{
    public static class PopulationFitnessHelper
    {
        public static void CalculateFitness(string objective, Population population)
        {
            foreach (var individual in population)
            {
                int genesValue = GetGenesValue(objective, individual);

                double fitness = Math.Pow(2, (double)genesValue / objective.Length) - 1;
                individual.Fitness = fitness * 100;
            }
        }

        private static int GetGenesValue(string objective, Individual individual)
        {
            var countEqualsGene = 0;
            for (int i = 0; i < individual.Identity.Length; i++)
            {
                if (individual.Identity[i] == objective[i])
                    countEqualsGene++;
            }

            return countEqualsGene;
        }
    }
}
