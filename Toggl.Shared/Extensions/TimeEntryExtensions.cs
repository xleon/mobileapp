using Toggl.Shared.Models;

namespace Toggl.Shared.Extensions
{
    public static class TimeEntryExtensions
    {
        public static bool IsRunning(this ITimeEntry self)
            => !self.Duration.HasValue;
    }
}
