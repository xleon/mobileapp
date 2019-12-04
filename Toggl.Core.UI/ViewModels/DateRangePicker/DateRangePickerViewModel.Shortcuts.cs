using System;
using System.Collections.Generic;
using Toggl.Core.Models;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public sealed partial class DateRangePickerViewModel
    {
        IEnumerable<DatePickerShortcut> createShortcuts(BeginningOfWeek beginningOfWeek, DateTimeOffset now)
        {
            var today = now.ToLocalTime().Date;

            yield return new TodayShortcut(today);
            yield return new YesterdayShortcut(today);
            yield return new ThisWeekShortcut(beginningOfWeek, today);
            yield return new LastWeekShortcut(beginningOfWeek, today);
            yield return new ThisMonthShortcut(today);
            yield return new LastMonthShortcut(today);
            yield return new ThisYearShortcut(today);
            yield return new LastYearShortcut(today);
        }

        private abstract class DatePickerShortcut
        {
            protected DateTime today;

            public DatePickerShortcut(DateTime today)
            {
                this.today = today;
            }

            public abstract ReportPeriod Period { get; }
            public abstract DateRange DateRange { get; }

            public virtual bool MatchesDateRange(DateRange range)
                => range == DateRange;
        }

        private class TodayShortcut : DatePickerShortcut
        {
            public override ReportPeriod Period
                => ReportPeriod.Today;

            public override DateRange DateRange
                => new DateRange(today, today);

            public TodayShortcut(DateTime today) : base(today)
            {
            }
        }

        private class YesterdayShortcut : DatePickerShortcut
        {
            public override ReportPeriod Period
                => ReportPeriod.Yesterday;

            public override DateRange DateRange
                => new DateRange(today.AddDays(-1), today.AddDays(-1));

            public YesterdayShortcut(DateTime today) : base(today)
            {
            }
        }

        private class ThisWeekShortcut : DatePickerShortcut
        {
            private BeginningOfWeek beginningOfWeek;

            public override ReportPeriod Period => ReportPeriod.ThisWeek;

            public override DateRange DateRange
            {
                get
                {
                    var beginning = today.BeginningOfWeek(beginningOfWeek);
                    var end = beginning.AddDays(6);
                    return new DateRange(beginning, end);
                }
            }

            public ThisWeekShortcut(BeginningOfWeek beginningOfWeek, DateTime today) : base(today)
            {
                this.beginningOfWeek = beginningOfWeek;
            }
        }

        private class LastWeekShortcut : DatePickerShortcut
        {
            private BeginningOfWeek beginningOfWeek;

            public override ReportPeriod Period => ReportPeriod.LastWeek;

            public override DateRange DateRange
            {
                get
                {
                    var beginning = today.BeginningOfWeek(beginningOfWeek).AddDays(-7);
                    var end = beginning.AddDays(6);
                    return new DateRange(beginning, end);
                }
            }

            public LastWeekShortcut(BeginningOfWeek beginningOfWeek, DateTime today) : base(today)
            {
                this.beginningOfWeek = beginningOfWeek;
            }
        }

        private class ThisMonthShortcut : DatePickerShortcut
        {
            public override ReportPeriod Period => ReportPeriod.ThisMonth;

            public override DateRange DateRange => new DateRange(
                today.FirstDayOfSameMonth(),
                today.LastDayOfSameMonth());

            public ThisMonthShortcut(DateTime today) : base(today)
            {
            }
        }

        private class LastMonthShortcut : DatePickerShortcut
        {
            public override ReportPeriod Period => ReportPeriod.LastMonth;

            public override DateRange DateRange
            {
                get
                {
                    var firstDayOfLastMonth = today.FirstDayOfSameMonth().AddMonths(-1);
                    var lastDayOfLastMonth = firstDayOfLastMonth.LastDayOfSameMonth();
                    return new DateRange(firstDayOfLastMonth, lastDayOfLastMonth);
                }
            }

            public LastMonthShortcut(DateTime today) : base(today)
            {
            }
        }

        private class ThisYearShortcut : DatePickerShortcut
        {
            public override ReportPeriod Period => ReportPeriod.ThisYear;

            public override DateRange DateRange
            {
                get
                {
                    var year = today.Year;
                    var firstDayOfYear = new DateTime(year, 1, 1);
                    var lastDayOfYear = new DateTime(year, 12, 31);
                    return new DateRange(firstDayOfYear, lastDayOfYear);
                }
            }

            public ThisYearShortcut(DateTime today) : base(today)
            {
            }
        }

        private class LastYearShortcut : DatePickerShortcut
        {
            public override ReportPeriod Period => ReportPeriod.LastYear;

            public override DateRange DateRange
            {
                get
                {
                    var year = today.Year - 1;
                    var firstDayOfLastYear = new DateTime(year, 1, 1);
                    var lastDayOfLastYear = new DateTime(year, 12, 31);
                    return new DateRange(firstDayOfLastYear, lastDayOfLastYear);
                }
            }

            public LastYearShortcut(DateTime today) : base(today)
            {
            }
        }
    }
}
