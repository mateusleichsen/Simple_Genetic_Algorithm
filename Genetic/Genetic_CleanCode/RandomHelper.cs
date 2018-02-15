using System;
using System.Collections.Generic;
using System.Text;

namespace Genetic
{
    public class RandomHelper
    {
        public static Random Generate(int seed) => new Random(seed);
        public static Random GenerateByDateTimeTicks()
        {
            var seed = (DateTime.Now.Second * 100) + DateTime.Now.Millisecond;
            Int32.TryParse(DateTime.Now.Ticks.ToString(), out seed);
            return Generate(seed);
        }
    }
}
