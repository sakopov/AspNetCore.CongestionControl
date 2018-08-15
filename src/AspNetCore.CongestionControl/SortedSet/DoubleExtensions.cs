using System;

namespace AspNetCore.CongestionControl.SortedSet
{
    public static class DoubleExtensions
    {
        public static bool NearlyEquals(this double value1, double value2, double tolerance = 0.0001)
        {
            if (value1 != value2)
            {
                return Math.Abs(value1 - value2) < tolerance;
            }

            return true;
        }
    }
}