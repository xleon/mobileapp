using System;
using Toggl.Shared;
using Toggl.Core.UI.Models;
using Toggl.Core.Models;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.Services
{
    public partial class CalendarShortcutsService
    {
        private class TodayCalendarShortcut : CalendarShortcut
        {
            public TodayCalendarShortcut(DateTime today)
            {
                Period = ReportPeriod.Today;

                DateRange = new DateRange(today, today);

                Text = Resources.Today;
            }
        }

        private class YesterdayCalendarShortcut : CalendarShortcut
        {
            public YesterdayCalendarShortcut(DateTime today)
            {
                var yesterday = today.AddDays(-1);
                DateRange = new DateRange(yesterday, yesterday);

                Period = ReportPeriod.Yesterday;

                Text = Resources.Yesterday;
            }
        }

        private class ThisWeekCalendarShortcut : CalendarShortcut
        {
            public ThisWeekCalendarShortcut(DateTime today, BeginningOfWeek beginningOfWeek)
            {
                var beginning = today.BeginningOfWeek(beginningOfWeek);
                var end = beginning.AddDays(6);
                DateRange = new DateRange(beginning, end);

                Period = ReportPeriod.ThisWeek;

                Text = Resources.ThisWeek;
            }
        }

        private class LastWeekCalendarShortcut : CalendarShortcut
        {
            public LastWeekCalendarShortcut(DateTime today, BeginningOfWeek beginningOfWeek)
            {
                var beginning = today.BeginningOfWeek(beginningOfWeek).AddDays(-7);
                var end = beginning.AddDays(6);
                DateRange = new DateRange(beginning, end);

                Period = ReportPeriod.LastWeek;

                Text = Resources.LastWeek;
            }
        }

        private class ThisMonthCalendarShortcut : CalendarShortcut
        {
            public ThisMonthCalendarShortcut(DateTime today)
            {
                var firstDayOfMonth = today.FirstDayOfSameMonth();
                var lastDayOfMonth = today.LastDayOfSameMonth();
                DateRange = new DateRange(firstDayOfMonth, lastDayOfMonth);

                Period = ReportPeriod.ThisMonth;

                Text = Resources.ThisMonth;
            }
        }

        private class LastMonthCalendarShortcut : CalendarShortcut
        {
            public LastMonthCalendarShortcut(DateTime today)
            {
                var firstDayOfMonth = today.FirstDayOfSameMonth().AddMonths(-1);
                var lastDayOfMonth = firstDayOfMonth.LastDayOfSameMonth();
                DateRange = new DateRange(firstDayOfMonth, lastDayOfMonth);

                Period = ReportPeriod.LastMonth;

                Text = Resources.LastMonth;
            }
        }

        private class ThisYearCalendarShortcut : CalendarShortcut
        {
            public ThisYearCalendarShortcut(DateTime today)
            {
                var year = today.Year;
                var firstDayOfYear = new DateTime(year, 1, 1);
                var lastDayOfYear = new DateTime(year, 12, 31);
                DateRange = new DateRange(firstDayOfYear, lastDayOfYear);

                Period = ReportPeriod.ThisYear;

                Text = Resources.ThisYear;
            }
        }

        private class LastYearCalendarShortcut : CalendarShortcut
        {
            public LastYearCalendarShortcut(DateTime today)
            {
                var year = today.Year - 1;
                var firstDayOfYear = new DateTime(year, 1, 1);
                var lastDayOfYear = new DateTime(year, 12, 31);
                DateRange = new DateRange(firstDayOfYear, lastDayOfYear);

                Period = ReportPeriod.LastYear;

                Text = Resources.LastYear;
            }
        }
    }
}
