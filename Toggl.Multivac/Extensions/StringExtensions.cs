using System;

namespace Toggl.Multivac.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoringCase(this string self, string value)
            => self.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
