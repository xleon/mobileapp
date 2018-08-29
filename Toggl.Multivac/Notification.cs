using System;

namespace Toggl.Multivac
{
    public struct Notification
    {
        public long Id { get; }

        public string Title { get; }

        public string Description { get; }

        public DateTimeOffset ScheduledTime { get; }

        public Notification(long id, string title, string description, DateTimeOffset scheduledTime)
        {
            Ensure.Argument.IsNotNullOrEmpty(title, nameof(title));

            Id = id;
            Title = title;
            Description = description;
            ScheduledTime = scheduledTime;
        }
    }
}
