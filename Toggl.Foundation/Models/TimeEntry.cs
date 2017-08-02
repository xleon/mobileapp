using System;

namespace Toggl.Foundation.Models
{
    internal partial class TimeEntry
    {
        internal sealed class Builder
        {
            private const string ErrorMessage = "You need to set the {0} before building a time entry";

            public static Builder Create() => new Builder();

            public bool IsDirty { get; private set; }

            public bool Billable { get; private set; }

            public string Description { get; private set; }
            
            public DateTimeOffset Start { get; private set; }

            private Builder() { }

            public TimeEntry Build() 
            {
                ensureValidity();
                return new TimeEntry(this);
            }

            public Builder SetStartDate(DateTimeOffset start)
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
                    throw new InvalidOperationException(string.Format(ErrorMessage, "start date"));

                if (Description == null)
                    throw new InvalidOperationException(string.Format(ErrorMessage, "description"));
            }
        }

        private TimeEntry(Builder builder)
        {
            Start = builder.Start;
            IsDirty = builder.IsDirty;
            Billable = builder.Billable;
            Description = builder.Description;
        }
    }
}
