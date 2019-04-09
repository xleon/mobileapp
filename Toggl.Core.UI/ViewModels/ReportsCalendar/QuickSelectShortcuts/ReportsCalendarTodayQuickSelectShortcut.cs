using Toggl.Core.Analytics;
using Toggl.Core.MvvmCross.Parameters;
using Toggl.Core.Services;

namespace Toggl.Core.MvvmCross.ViewModels.ReportsCalendar.QuickSelectShortcuts
{
    public sealed class ReportsCalendarTodayQuickSelectShortcut : ReportsCalendarBaseQuickSelectShortcut
    {
        public ReportsCalendarTodayQuickSelectShortcut(ITimeService timeService)
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
