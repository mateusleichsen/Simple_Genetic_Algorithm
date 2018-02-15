using System;
using System.Collections.Generic;
using System.Text;

namespace Genetic
{
    public class IndividualComparer : IComparer<Individual>
    {
        public int Compare(Individual x, Individual y)
        {
            if (x.Fitness > y.Fitness)
                return 1;
            if (x.Fitness < y.Fitness)
                return -1;

            return 0;
        }
    }
}
