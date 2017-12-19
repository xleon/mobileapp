using System;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class DateRangeParameter
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public static DateRangeParameter WithDates(
            DateTimeOffset start, DateTimeOffset end)
        {
            if (start > end)
                (start, end) = (end, start);

            return new DateRangeParameter { StartDate = start, EndDate = end };
        }
    }
}
