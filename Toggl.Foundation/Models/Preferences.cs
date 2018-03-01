using System;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Models
{
    internal sealed partial class Preferences
    {
        internal sealed class Builder
        {
            public static Builder FromExisting(IDatabasePreferences preferences)
                => new Builder(preferences);

            public DateFormat DateFormat { get; private set; }

            public TimeFormat TimeOfDayFormat { get; private set; }

            public DurationFormat DurationFormat { get; private set; }

            public bool CollapseTimeEntries { get; private set; }

            private Builder(IDatabasePreferences preferences)
            {
                DateFormat = preferences.DateFormat;
                TimeOfDayFormat = preferences.TimeOfDayFormat;
                DurationFormat = preferences.DurationFormat;
                CollapseTimeEntries = preferences.CollapseTimeEntries;
            }

            public Preferences Build()
            {
                ensureValidity();
                return new Preferences(this);
            }

            public Builder SetDateFormat(DateFormat dateFormat)
            {
                DateFormat = dateFormat;
                return this;
            }

            public Builder SetTimeOfDayFormat(TimeFormat timeFormat)
            {
                TimeOfDayFormat = timeFormat;
                return this;
            }

            public Builder SetDurationFormat(DurationFormat durationFormat)
            {
                DurationFormat = durationFormat;
                return this;
            }

            public Builder SetCollapseTimeEntries(bool collapseTimeEntries)
            {
                CollapseTimeEntries = collapseTimeEntries;
                return this;
            }

            private void ensureValidity()
            {
                if (Enum.IsDefined(typeof(DurationFormat), DurationFormat) == false)
                    throw new InvalidOperationException($"You need to set a valid value to the {nameof(DurationFormat)} property before building preferences.");
            }
        }

        private Preferences(Builder builder)
        {
            DateFormat = builder.DateFormat;
            TimeOfDayFormat = builder.TimeOfDayFormat;
            DurationFormat = builder.DurationFormat;
            CollapseTimeEntries = builder.CollapseTimeEntries;
        }
    }
}
