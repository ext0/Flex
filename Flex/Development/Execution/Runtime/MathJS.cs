using Flex.Development.Execution.Runtime.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    [DynamicExposedClass]
    public class MathJS
    {
        private Random _random;

        public MathJS()
        {
            _random = new Random();
        }

        private delegate double MaxDelegate(double a, double b);

        [DynamicExposedMethod(typeof(MaxDelegate), "max")]
        public double Max(double a, double b)
        {
            return Math.Max(a, b);
        }

        private delegate double MinDelegate(double a, double b);

        [DynamicExposedMethod(typeof(MinDelegate), "min")]
        public double Min(double a, double b)
        {
            return Math.Min(a, b);
        }

        private delegate int RandomDelegate(int lower, int upper);

        [DynamicExposedMethod(typeof(RandomDelegate), "random")]
        public int Random(int a, int b)
        {
            return _random.Next(a, b);
        }

    }
}
