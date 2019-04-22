using System;

namespace Toggl.iOS.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Positive(this TimeSpan duration)
            => duration < TimeSpan.Zero ? duration.Negate() : duration;
    }
}
