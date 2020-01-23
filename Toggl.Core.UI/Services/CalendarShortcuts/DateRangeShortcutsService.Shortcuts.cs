using System;
using Toggl.Shared;
using Toggl.Core.UI.Models;
using Toggl.Core.Models;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.Services
{
    public partial class DateRangeShortcutsService
    {
        private class TodayDateRangeShortcut : DateRangeShortcut
        {
            public TodayDateRangeShortcut(DateTime today)
            {
                Period = DateRangePeriod.Today;

                DateRange = new DateRange(today, today);

                Text = Resources.Today;
            }
        }

        private class YesterdayDateRangeShortcut : DateRangeShortcut
        {
            public YesterdayDateRangeShortcut(DateTime today)
            {
                var yesterday = today.AddDays(-1);
                DateRange = new DateRange(yesterday, yesterday);

                Period = DateRangePeriod.Yesterday;

                Text = Resources.Yesterday;
            }
        }

        private class ThisWeekDateRangeShortcut : DateRangeShortcut
        {
            public ThisWeekDateRangeShortcut(DateTime today, BeginningOfWeek beginningOfWeek)
            {
                var beginning = today.BeginningOfWeek(beginningOfWeek);
                var end = beginning.AddDays(6);
                DateRange = new DateRange(beginning, end);

                Period = DateRangePeriod.ThisWeek;

                Text = Resources.ThisWeek;
            }
        }

        private class LastWeekDateRangeShortcut : DateRangeShortcut
        {
            public LastWeekDateRangeShortcut(DateTime today, BeginningOfWeek beginningOfWeek)
            {
                var beginning = today.BeginningOfWeek(beginningOfWeek).AddDays(-7);
                var end = beginning.AddDays(6);
                DateRange = new DateRange(beginning, end);

                Period = DateRangePeriod.LastWeek;

                Text = Resources.LastWeek;
            }
        }

        private class ThisMonthDateRangeShortcut : DateRangeShortcut
        {
            public ThisMonthDateRangeShortcut(DateTime today)
            {
                var firstDayOfMonth = today.FirstDayOfSameMonth();
                var lastDayOfMonth = today.LastDayOfSameMonth();
                DateRange = new DateRange(firstDayOfMonth, lastDayOfMonth);

                Period = DateRangePeriod.ThisMonth;

                Text = Resources.ThisMonth;
            }
        }

        private class LastMonthDateRangeShortcut : DateRangeShortcut
        {
            public LastMonthDateRangeShortcut(DateTime today)
            {
                var firstDayOfMonth = today.FirstDayOfSameMonth().AddMonths(-1);
                var lastDayOfMonth = firstDayOfMonth.LastDayOfSameMonth();
                DateRange = new DateRange(firstDayOfMonth, lastDayOfMonth);

                Period = DateRangePeriod.LastMonth;

                Text = Resources.LastMonth;
            }
        }

        private class ThisYearDateRangeShortcut : DateRangeShortcut
        {
            public ThisYearDateRangeShortcut(DateTime today)
            {
                var year = today.Year;
                var firstDayOfYear = new DateTime(year, 1, 1);
                var lastDayOfYear = new DateTime(year, 12, 31);
                DateRange = new DateRange(firstDayOfYear, lastDayOfYear);

                Period = DateRangePeriod.ThisYear;

                Text = Resources.ThisYear;
            }
        }

        private class LastYearDateRangeShortcut : DateRangeShortcut
        {
            public LastYearDateRangeShortcut(DateTime today)
            {
                var year = today.Year - 1;
                var firstDayOfYear = new DateTime(year, 1, 1);
                var lastDayOfYear = new DateTime(year, 12, 31);
                DateRange = new DateRange(firstDayOfYear, lastDayOfYear);

                Period = DateRangePeriod.LastYear;

                Text = Resources.LastYear;
            }
        }
    }
}
