using System;
using System.Collections.Generic;
using System.Text;

namespace Genetic
{
    public static class IndividualListExtension
    {
        public static Population ToPopulation(this IEnumerable<Individual> list)
        {
            return new Population(list);
        }
    }
}
