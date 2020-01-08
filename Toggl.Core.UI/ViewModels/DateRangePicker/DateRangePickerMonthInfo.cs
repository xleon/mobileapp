using System;
using Toggl.Core.UI.Helper;
using Toggl.Shared;
using SelectionState = Toggl.Shared.Either<System.DateTime, Toggl.Shared.DateRange>;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public class DateRangePickerMonthInfo
    {
        private const int daysInWeek = 7;

        public int Month { get; }

        public int Year { get; }

        public string MonthDisplay
            => new DateTime(Year, Month, 1)
            .ToString(DateFormatCultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern);

        public bool IsTodaySelected
            => Selection.HasValue
            && Today.HasValue
            && Selection.Value.Contains(Today.Value);

        public int RowCount
            => DisplayDates.Length / 7;

        public bool IsDateTheFirstSelectedDate(DateTime date)
            => date == Selection?.Beginning && IsSelectionBeginningBoundary;

        public bool IsDateTheLastSelectedDate(DateTime date)
            => date == Selection?.End && IsSelectionEndBoundary;

        /// <summary>
        /// If this is the current month, Today will be set to this value.
        /// It will be null for all other months.
        /// </summary>
        public DateTime? Today { get; }

        /// <summary>
        /// All dates that should be displayed for a month.
        /// For every month it includes a few days of previous or next month
        /// as defined by the BeginningOfWeek setting.
        /// </summary>
        public DateRange DisplayDates { get; }

        /// <summary>
        /// The range of days that should be selected in this month.
        /// If this value is null for a month, nothing should be selected in that month.
        /// </summary>
        public DateRange? Selection { get; }

        /// <summary>
        /// Defines whether the selection is partial, where only the
        /// beginning of the range is selected, awaiting the selection of the end date.
        /// If it is true, the Selection's Length will be 1.
        /// If Selection is null, this property should not be read.
        /// </summary>
        public bool IsSelectionPartial { get; private set; }

        /// <summary>
        /// This property defines whether the beginning of the Selection is the
        /// left boundary of the total user selection and can be used to represent
        /// such dates differently on UI.
        /// If Selection is null, this property should not be read.
        /// </summary>
        public bool IsSelectionBeginningBoundary { get; }

        /// <summary>
        /// This property defines whether the end of the Selection is the
        /// left boundary of the total user selection and can be used to represent
        /// such dates differently on UI.
        /// If Selection is null, this property should not be read.
        /// </summary>
        public bool IsSelectionEndBoundary { get; }

        private DateRangePickerMonthInfo(
            DateTime firstDayOfMonth,
            DateRange displayedDates,
            DateTime? today,
            DateRange? selection,
            bool isSelectionPartial,
            bool isSelectionBeginningBoundary,
            bool isSelectionEndBoundary)
        {
            Month = firstDayOfMonth.Month;
            Year = firstDayOfMonth.Year;
            DisplayDates = displayedDates;
            Today = today;
            Selection = selection;
            IsSelectionPartial = isSelectionPartial;
            IsSelectionBeginningBoundary = isSelectionBeginningBoundary;
            IsSelectionEndBoundary = isSelectionEndBoundary;
        }

        public static DateRangePickerMonthInfo Create(
            DateTime firstDayOfMonth,
            SelectionState selection,
            BeginningOfWeek beginningOfWeek)
        {
            var month = DateRange.MonthFrom(firstDayOfMonth);

            var preMonthDaysCount = ((int)firstDayOfMonth.DayOfWeek - (int)beginningOfWeek + daysInWeek) % daysInWeek;
            var postMonthDaysCount = daysInWeek - (month.Length + preMonthDaysCount) % daysInWeek;
            postMonthDaysCount %= 7;

            var displayedBeginning = month.Beginning.AddDays(-preMonthDaysCount);
            var displayedEnd = month.End.AddDays(postMonthDaysCount);

            var displayedRange = new DateRange(displayedBeginning, displayedEnd);

            var today = displayedRange.Contains(DateTime.Today)
                ? DateTime.Today
                : (DateTime?)null;

            return selection.Match(
                beginning => getMonthInfoForSingleDate(firstDayOfMonth, beginning, displayedRange, today),
                range => getMonthInfoForRange(firstDayOfMonth, range, displayedRange, displayedRange, today));
        }

        private static DateRangePickerMonthInfo getMonthInfoForRange(
            DateTime firstDayOfMonth,
            DateRange selectedRange,
            DateRange displayedRange,
            DateRange displayedDates,
            DateTime? today)
        {
            if (!displayedRange.OverlapsWith(selectedRange))
                return new DateRangePickerMonthInfo(firstDayOfMonth, displayedDates, today, null, false, false, false);

            var displayedSelectionDateBeginning = selectedRange.Beginning > displayedRange.Beginning
                ? selectedRange.Beginning
                : displayedRange.Beginning;

            var displayedSelectionDateEnd = selectedRange.End < displayedRange.End
                ? selectedRange.End
                : displayedRange.End;

            var displaySelection = new DateRange(displayedSelectionDateBeginning, displayedSelectionDateEnd);

            var isSelectionBeginningBoundary = selectedRange.Beginning == displayedSelectionDateBeginning;
            var isSelectionEndBoundary = selectedRange.End == displayedSelectionDateEnd;

            return new DateRangePickerMonthInfo(
                firstDayOfMonth,
                displayedDates,
                today,
                displaySelection,
                isSelectionPartial: false,
                isSelectionBeginningBoundary: isSelectionBeginningBoundary,
                isSelectionEndBoundary: isSelectionEndBoundary);
        }

        private static DateRangePickerMonthInfo getMonthInfoForSingleDate(
            DateTime firstDayOfMonth,
            DateTime selectedBeginning,
            DateRange displayedDates,
            DateTime? today)
        {
            var isSelectionDisplayed = displayedDates.Contains(selectedBeginning);

            var selectedDateRange = isSelectionDisplayed
                ? new DateRange(selectedBeginning, selectedBeginning)
                : (DateRange?)null;

            return new DateRangePickerMonthInfo(
                firstDayOfMonth,
                displayedDates,
                today,
                selectedDateRange,
                isSelectionPartial: true,
                isSelectionBeginningBoundary: isSelectionDisplayed,
                isSelectionEndBoundary: isSelectionDisplayed);
        }
    }
}
