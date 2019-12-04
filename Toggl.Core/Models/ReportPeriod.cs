using Toggl.Shared;

namespace Toggl.Core.Models
{
    public enum ReportPeriod
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

    public static class ReportPeriodExtensions
    {
        public static string ToHumanReadableString(this ReportPeriod period)
        {
            return period switch
            {
                ReportPeriod.LastMonth => Resources.LastMonth,
                ReportPeriod.LastWeek => Resources.LastWeek,
                ReportPeriod.Yesterday => Resources.Yesterday,
                ReportPeriod.Today => Resources.Today,
                ReportPeriod.ThisWeek => Resources.ThisWeek,
                ReportPeriod.ThisMonth => Resources.ThisMonth,
                ReportPeriod.ThisYear => Resources.ThisYear,
                ReportPeriod.LastYear => Resources.LastYear,
                _ => Resources.Unknown,
            };
        }
    }
}
