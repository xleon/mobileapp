using System;

namespace Toggl.Multivac.Extensions
{
    public static class ClampExtension
    {
        public static T Clamp<T>(this T num, T min, T max) where T : IComparable
        {
            if (num.CompareTo(min) < 0) return min;
            if (num.CompareTo(max) > 0) return max;
            return num;
        }
    }
}
