using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    public static class Random
    {
        private static int _seed;
        private static System.Random _random;

        static Random()
        {
            _random = new System.Random();
            _seed = _random.Next();
            _random = new System.Random(_seed);
        }

        static void seed(int seed)
        {
            _seed = seed;
            _random = new System.Random(_seed);
        }

        public static double next()
        {
            return (double)_random.Next() / int.MaxValue;
        }
    }
}
