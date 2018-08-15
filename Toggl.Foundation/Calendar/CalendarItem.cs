using System;
using Toggl.Foundation.Models.Interfaces;
using Toggl.PrimeRadiant;
using ColorHelper = Toggl.Foundation.Helper.Color;

namespace Toggl.Foundation.Calendar
{
    public struct CalendarItem
    {
        public CalendarItemSource Source { get; }

        public TimeSpan Duration { get; }

        public DateTimeOffset StartTime { get; }

        public DateTimeOffset EndTime => StartTime + Duration;

        public string Description { get; }

        public string Color { get; }

        public long? TimeEntryId { get; }

        public string CalendarId { get; }

        public bool CanBeSynced { get; }

        public bool IsSynced { get; }

        public CalendarItem(
            CalendarItemSource source,
            DateTimeOffset startTime,
            TimeSpan duration,
            string description,
            string color = "",
            long? timeEntryId = null,
            string calendarId = "")
        {
            Source = source;
            StartTime = startTime;
            Duration = duration;
            Description = description;
            Color = color;
            TimeEntryId = timeEntryId;
            CalendarId = calendarId;
            IsSynced = true;
            CanBeSynced = true;
        }

        private CalendarItem(IThreadSafeTimeEntry timeEntry)
            : this(
                CalendarItemSource.TimeEntry,
                timeEntry.Start,
                TimeSpan.FromSeconds(timeEntry.Duration ?? 0),
                timeEntry.Description,
                timeEntry.Project?.Color ?? ColorHelper.NoProject,
                timeEntry.Id)
        {
            IsSynced = timeEntry.SyncStatus == SyncStatus.InSync;
            CanBeSynced = timeEntry.SyncStatus != SyncStatus.SyncFailed;
        }

        public static CalendarItem From(IThreadSafeTimeEntry timeEntry)
            => new CalendarItem(timeEntry);
    }
}
