using Toggl.Multivac.Models;

namespace Toggl.Multivac.Extensions
{
    public static class TimeEntryExtensions
    {
        public static bool IsRunning(this ITimeEntry self)
            => !self.Duration.HasValue;
    }
}
