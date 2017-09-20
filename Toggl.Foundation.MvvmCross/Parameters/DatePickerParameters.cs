using System;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    [Preserve(AllMembers = true)]
    public sealed class DatePickerParameters
    {
        public DateTimeOffset CurrentDate { get; set; }

        public DateTimeOffset MinDate { get; set; }

        public DateTimeOffset MaxDate { get; set; }

        public static DatePickerParameters WithDates(DateTimeOffset current, DateTimeOffset min, DateTimeOffset max) => 
            new DatePickerParameters
            {
                CurrentDate = current,
                MinDate = min,
                MaxDate = max
            };
    }
}
