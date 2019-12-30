using System;
using Toggl.Shared.Models;

namespace Toggl.Shared.Extensions
{
    public static class TimeEntryExtensions
    {
        public static bool IsRunning(this ITimeEntry self)
            => !self.Duration.HasValue;

        public static TimeSpan? TimeSpanDuration(this ITimeEntry timeEntry)
            => timeEntry.Duration.HasValue
                ? TimeSpan.FromSeconds(timeEntry.Duration.Value)
                : (TimeSpan?)null;

        public static DateTimeOffset? StopTime(this ITimeEntry timeEntry)
            => timeEntry.Duration.HasValue
                ? timeEntry.Start + TimeSpan.FromSeconds(timeEntry.Duration.Value)
                : (DateTimeOffset?)null;
    }
}
