using System;
using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.UI.Parameters;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts
{
    public sealed class ReportsCalendarThisMonthQuickSelectShortcut
        : ReportsCalendarBaseQuickSelectShortcut
    {
        public ReportsCalendarThisMonthQuickSelectShortcut(ITimeService timeService)
            : base(timeService, Resources.ThisMonth, ReportPeriod.ThisMonth)
        {
        }

        public override ReportsDateRange GetDateRange()
        {
            var start = TimeService.CurrentDateTime.RoundDownToLocalMonth();
            var end = start.AddMonths(1).AddDays(-1);
            return ReportsDateRange
                .WithDates(start, end)
                .WithSource(ReportsSource.ShortcutThisMonth);
        }
    }
}
