using Microsoft.ClearScript;
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

        [ScriptMember(ScriptAccess.ReadOnly)]
        static void seed(int seed)
        {
            _seed = seed;
            _random = new System.Random(_seed);
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public static double next()
        {
            return (double)_random.Next() / int.MaxValue;
        }

        [ScriptMember(ScriptAccess.ReadOnly)]
        public static int next(int min, int max)
        {
            return _random.Next(min, max + 1);
        }
    }
}
