using System;
using System.Collections.Generic;
using Toggl.Core.Models;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public sealed partial class DateRangePickerViewModel
    {
        IEnumerable<DateRangePickerShortcut> createShortcuts(BeginningOfWeek beginningOfWeek, DateTimeOffset now)
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

        public struct Shortcut : IDiffableByIdentifier<Shortcut>
        {
            public static Shortcut From(DateRangePickerShortcut shortcut, bool isSelected)
                => new Shortcut(shortcut.Period, shortcut.Text, isSelected);

            private Shortcut(ReportPeriod period, string text, bool isSelected)
            {
                ReportPeriod = period;
                Text = text;
                IsSelected = isSelected;
            }

            public string Text { get; }
            public ReportPeriod ReportPeriod { get; }
            public bool IsSelected { get; }

            public long Identifier
                => (long)ReportPeriod;

            public bool Equals(Shortcut other)
                => other.ReportPeriod == ReportPeriod
                && other.IsSelected == IsSelected;
        }

        public abstract class DateRangePickerShortcut 
        {
            protected DateTime today;

            public DateRangePickerShortcut(DateTime today)
            {
                this.today = today;
            }

            public abstract string Text { get; }
            public abstract ReportPeriod Period { get; }
            public abstract DateRange DateRange { get; }

            public virtual bool MatchesDateRange(DateRange range)
                => range == DateRange;
        }

        private class TodayShortcut : DateRangePickerShortcut
        {
            public override ReportPeriod Period
                => ReportPeriod.Today;

            public override DateRange DateRange
                => new DateRange(today, today);

            public override string Text
                => Resources.Today;

            public TodayShortcut(DateTime today) : base(today)
            {
            }
        }

        private class YesterdayShortcut : DateRangePickerShortcut
        {
            public override ReportPeriod Period
                => ReportPeriod.Yesterday;

            public override DateRange DateRange
                => new DateRange(today.AddDays(-1), today.AddDays(-1));

            public override string Text
                => Resources.Yesterday;

            public YesterdayShortcut(DateTime today) : base(today)
            {
            }
        }

        private class ThisWeekShortcut : DateRangePickerShortcut
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

            public override string Text
                => Resources.ThisWeek;

            public ThisWeekShortcut(BeginningOfWeek beginningOfWeek, DateTime today) : base(today)
            {
                this.beginningOfWeek = beginningOfWeek;
            }
        }

        private class LastWeekShortcut : DateRangePickerShortcut
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

            public override string Text
                => Resources.LastWeek;

            public LastWeekShortcut(BeginningOfWeek beginningOfWeek, DateTime today) : base(today)
            {
                this.beginningOfWeek = beginningOfWeek;
            }
        }

        private class ThisMonthShortcut : DateRangePickerShortcut
        {
            public override ReportPeriod Period => ReportPeriod.ThisMonth;

            public override DateRange DateRange => new DateRange(
                today.FirstDayOfSameMonth(),
                today.LastDayOfSameMonth());

            public override string Text
                => Resources.ThisMonth;

            public ThisMonthShortcut(DateTime today) : base(today)
            {
            }
        }

        private class LastMonthShortcut : DateRangePickerShortcut
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

            public override string Text
                => Resources.LastMonth;

            public LastMonthShortcut(DateTime today) : base(today)
            {
            }
        }

        private class ThisYearShortcut : DateRangePickerShortcut
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

            public override string Text
                => Resources.ThisYear;

            public ThisYearShortcut(DateTime today) : base(today)
            {
            }
        }

        private class LastYearShortcut : DateRangePickerShortcut
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

            public override string Text
                => Resources.LastYear;

            public LastYearShortcut(DateTime today) : base(today)
            {
            }
        }
    }
}
