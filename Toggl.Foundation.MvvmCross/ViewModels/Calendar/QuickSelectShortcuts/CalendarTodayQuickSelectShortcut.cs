using Toggl.Foundation.Analytics;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.Services;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar.QuickSelectShortcuts
{
    public sealed class CalendarTodayQuickSelectShortcut : CalendarBaseQuickSelectShortcut
    {
        public CalendarTodayQuickSelectShortcut(ITimeService timeService)
            : base(timeService, Resources.Today, ReportPeriod.Today)
        {
        }

        public override ReportsDateRangeParameter GetDateRange()
        {
            var today = TimeService.CurrentDateTime.Date;
            return ReportsDateRangeParameter
                .WithDates(today, today)
                .WithSource(ReportsSource.ShortcutToday);
        }
    }
}
