using System;
using Toggl.Core.Analytics;

namespace Toggl.Core.UI.Parameters
{
    public sealed class ReportsDateRangeParameter
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public DateRangeSelectionSource Source { get; set; }

        public static ReportsDateRangeParameter WithDates(
            DateTimeOffset start,
            DateTimeOffset end
        )
        {
            if (start > end)
                (start, end) = (end, start);

            return new ReportsDateRangeParameter { StartDate = start, EndDate = end, Source = DateRangeSelectionSource.Other };
        }

        public ReportsDateRangeParameter WithSource(DateRangeSelectionSource source)
        {
            return new ReportsDateRangeParameter { StartDate = this.StartDate, EndDate = this.EndDate, Source = source };
        }
    }
}
