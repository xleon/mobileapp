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

        public CalendarIconKind IconKind { get; }

        public CalendarItem(
            CalendarItemSource source,
            DateTimeOffset startTime,
            TimeSpan duration,
            string description,
            CalendarIconKind iconKind,
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
            IconKind = iconKind;
        }

        private CalendarItem(IThreadSafeTimeEntry timeEntry)
            : this(
                CalendarItemSource.TimeEntry,
                timeEntry.Start,
                TimeSpan.FromSeconds(timeEntry.Duration ?? 0),
                timeEntry.Description,
                CalendarIconKind.None,
                timeEntry.Project?.Color ?? ColorHelper.NoProject,
                timeEntry.Id)
        {
            switch (timeEntry.SyncStatus)
            {
                case SyncStatus.SyncNeeded:
                    IconKind = CalendarIconKind.Unsynced;
                    break;
                case SyncStatus.SyncFailed:
                    IconKind = CalendarIconKind.Unsyncable;
                    break;
            }
        }

        public static CalendarItem From(IThreadSafeTimeEntry timeEntry)
            => new CalendarItem(timeEntry);
    }
}
