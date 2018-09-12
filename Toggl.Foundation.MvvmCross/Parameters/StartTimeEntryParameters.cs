using System;
using Toggl.Foundation;
using static Toggl.Foundation.Helper.Constants;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class StartTimeEntryParameters
    {
        private static readonly TimeSpan defaultManualModeDuration = TimeSpan.FromMinutes(DefaultTimeEntryDurationForManualModeInMinutes);

        public DateTimeOffset StartTime { get; }

        public string PlaceholderText { get; }

        public TimeSpan? Duration { get; }

        public long? WorkspaceId { get; }

        public StartTimeEntryParameters(DateTimeOffset startTime, string placeholderText, TimeSpan? duration, long? workspaceId)
        {
            StartTime = startTime;
            PlaceholderText = placeholderText;
            Duration = duration;
            WorkspaceId = workspaceId;
        }

        public static StartTimeEntryParameters ForManualMode(DateTimeOffset now)
            => new StartTimeEntryParameters(
                now.Subtract(defaultManualModeDuration),
                Resources.ManualTimeEntryPlaceholder,
                defaultManualModeDuration,
                null);
        
        public static StartTimeEntryParameters ForTimerMode(DateTimeOffset now)
            => new StartTimeEntryParameters(now, Resources.StartTimeEntryPlaceholder, null, null);
    }
}
