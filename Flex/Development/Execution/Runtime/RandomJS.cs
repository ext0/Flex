using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    public class Random
    {
        private int _seed;
        private System.Random _random;

        public Random()
        {
            _random = new System.Random();
            _seed = _random.Next();
        }

        public Random(int seed)
        {
            _seed = seed;
        }

        public int next()
        {
            return _random.Next();
        }
    }
}
