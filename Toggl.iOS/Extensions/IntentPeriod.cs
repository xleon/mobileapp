using Toggl.Core.Models;
using Toggl.iOS.Intents;

namespace Toggl.iOS.Extensions
{
    public static class IntentPeriodExtension
    {
        public static ShowReportPeriodReportPeriod ToShowReportPeriodReportPeriod(this DateRangePeriod p)
        {
            switch (p)
            {
                case DateRangePeriod.LastMonth:
                    return ShowReportPeriodReportPeriod.LastMonth;
                case DateRangePeriod.LastWeek:
                    return ShowReportPeriodReportPeriod.LastWeek;
                case DateRangePeriod.Yesterday:
                    return ShowReportPeriodReportPeriod.Yesterday;
                case DateRangePeriod.Today:
                    return ShowReportPeriodReportPeriod.Today;
                case DateRangePeriod.ThisWeek:
                    return ShowReportPeriodReportPeriod.ThisWeek;
                case DateRangePeriod.ThisMonth:
                    return ShowReportPeriodReportPeriod.ThisMonth;
                case DateRangePeriod.ThisYear:
                    return ShowReportPeriodReportPeriod.ThisYear;
                case DateRangePeriod.LastYear:
                    return ShowReportPeriodReportPeriod.LastYear;
                default:
                    return ShowReportPeriodReportPeriod.Unknown;
            }
        }
        public static DateRangePeriod ToDateRangePeriod(this ShowReportPeriodReportPeriod intentPeriod)
        {
            switch (intentPeriod)
            {
                case ShowReportPeriodReportPeriod.Today:
                    return DateRangePeriod.Today;
                case ShowReportPeriodReportPeriod.Yesterday:
                    return DateRangePeriod.Yesterday;
                case ShowReportPeriodReportPeriod.LastMonth:
                    return DateRangePeriod.LastMonth;
                case ShowReportPeriodReportPeriod.ThisMonth:
                    return DateRangePeriod.ThisMonth;
                case ShowReportPeriodReportPeriod.LastWeek:
                    return DateRangePeriod.LastWeek;
                case ShowReportPeriodReportPeriod.ThisWeek:
                    return DateRangePeriod.ThisWeek;
                case ShowReportPeriodReportPeriod.ThisYear:
                    return DateRangePeriod.ThisYear;
                case ShowReportPeriodReportPeriod.LastYear:
                    return DateRangePeriod.LastYear;
                default:
                    return DateRangePeriod.Unknown;
            }
        }
    }
}
