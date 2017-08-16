using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Models
{
    internal partial class TimeEntry
    {
        internal sealed class Builder
        {
            private const string errorMessage = "You need to set the {0} before building a time entry";

            public static Builder Create(long id) => new Builder(id);

            public long Id { get; }

            public bool IsDirty { get; private set; }

            public bool Billable { get; private set; }

            public string Description { get; private set; }

            public DateTimeOffset Start { get; private set; }

            private Builder(long id) 
            {
                Id = id;
            }

            public TimeEntry Build()
            {
                ensureValidity();
                return new TimeEntry(this);
            }

            public Builder SetStart(DateTimeOffset start)
            {
                Start = start;
                return this;
            }

            public Builder SetIsDirty(bool isDirty)
            {
                IsDirty = isDirty;
                return this;
            }

            public Builder SetDescription(string description)
            {
                Description = description;
                return this;
            }

            public Builder SetBillable(bool billable)
            {
                Billable = billable;
                return this;
            }

            private void ensureValidity()
            {
                if (Start == default(DateTimeOffset))
                    throw new InvalidOperationException(string.Format(errorMessage, "start date"));

                if (Description == null)
                    throw new InvalidOperationException(string.Format(errorMessage, "description"));
            }
        }

        public TimeEntry(IDatabaseTimeEntry timeEntry, DateTimeOffset stop)
            : this(timeEntry, true)
        {
            if (Start > stop)
                throw new ArgumentOutOfRangeException(nameof(stop), "The stop date must be equal to or greater than the start date");

            Stop = stop;
        }

        private TimeEntry(Builder builder)
        {
            Id = builder.Id;
            Start = builder.Start;
            IsDirty = builder.IsDirty;
            Billable = builder.Billable;
            Description = builder.Description;
        }
    }

    internal static class TimeEntryExtensions
    {
        public static TimeEntry With(this IDatabaseTimeEntry self, DateTimeOffset stop) => new TimeEntry(self, stop);
    }
}
