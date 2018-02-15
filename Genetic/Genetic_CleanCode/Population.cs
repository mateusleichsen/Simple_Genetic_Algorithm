using System;
using System.Collections.Generic;
using System.Text;

namespace Genetic
{
    public class Population : List<Individual>
    {
        public Population()
        {
        }

        public Population(IEnumerable<Individual> collection) : base(collection)
        {
        }

        public Population(int capacity) : base(capacity)
        {
        }
    }
}
