using Toggl.Shared;

namespace Toggl.Core.Models
{
    public enum DateRangePeriod
    {
        Unknown = 0,
        Today,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisYear,
        LastYear
    }

    public static class DateRangePeriodExtensions
    {
        public static string ToHumanReadableString(this DateRangePeriod period)
        {
            return period switch
            {
                DateRangePeriod.LastMonth => Resources.LastMonth,
                DateRangePeriod.LastWeek => Resources.LastWeek,
                DateRangePeriod.Yesterday => Resources.Yesterday,
                DateRangePeriod.Today => Resources.Today,
                DateRangePeriod.ThisWeek => Resources.ThisWeek,
                DateRangePeriod.ThisMonth => Resources.ThisMonth,
                DateRangePeriod.ThisYear => Resources.ThisYear,
                DateRangePeriod.LastYear => Resources.LastYear,
                _ => Resources.Unknown,
            };
        }
    }
}
