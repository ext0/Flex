using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Execution.Runtime
{
    public static class Math
    {
        public static double E = 2.718281828459045;
        public static double PI = 3.141592653589793;
        public static double SQRT2 = 1.4142135623730951;
        public static double SQRT1_2 = 0.7071067811865476;
        public static double LN2 = 0.6931471805599453;
        public static double LN10 = 2.302585092994046;
        public static double LOG2E = 1.4426950408889634;
        public static double LOG10E = 0.4342944819032518;

        public static double abs(double x)
        {
            return System.Math.Abs(x);
        }

        public static double acos(double x)
        {
            return System.Math.Acos(x);
        }

        public static double asin(double x)
        {
            return System.Math.Asin(x);
        }

        public static double atan(double x)
        {
            return System.Math.Atan(x);
        }

        public static double atan2(double x, double y)
        {
            return System.Math.Atan2(x, y);
        }

        public static double ceil(double x)
        {
            return System.Math.Ceiling(x);
        }

        public static double cos(double x)
        {
            return System.Math.Cos(x);
        }

        public static double exp(double x)
        {
            return System.Math.Exp(x);
        }

        public static double floor(double x)
        {
            return System.Math.Floor(x);
        }

        public static double log(double x)
        {
            return System.Math.Log(x);
        }

        public static double log(double x, double y)
        {
            return System.Math.Log(y, x);
        }

        public static double max(double x, double y)
        {
            return System.Math.Max(x, y);
        }

        public static double min(double x, double y)
        {
            return System.Math.Min(x, y);
        }

        public static double pow(double x, double y)
        {
            return System.Math.Pow(x, y);
        }

        public static double round(double x)
        {
            return System.Math.Round(x);
        }

        public static double sin(double x)
        {
            return System.Math.Sin(x);
        }

        public static double sqrt(double x)
        {
            return System.Math.Sqrt(x);
        }

        public static double tan(double x)
        {
            return System.Math.Tan(x);
        }
    }
}
