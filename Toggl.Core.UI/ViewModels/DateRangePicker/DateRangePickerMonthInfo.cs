using System;
using Toggl.Shared;
using SelectionState = Toggl.Shared.Either<System.DateTime, Toggl.Shared.DateRange>;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public class DateRangePickerMonthInfo
    {
        private const int daysInWeek = 7;

        public int Month { get; }

        public int Year { get; }

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
            DateTime? today = null,
            DateRange? selection = null,
            bool isSelectionBeginningBoundary = false,
            bool isSelectionEndBoundary = false)
        {
            Month = firstDayOfMonth.Month;
            Year = firstDayOfMonth.Year;
            DisplayDates = displayedDates;
            Today = today;
            Selection = selection;
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

            var displayedBeginning = month.Beginning.AddDays(-preMonthDaysCount);
            var displayedEnd = month.End.AddDays(postMonthDaysCount);

            var displayedDates = new DateRange(displayedBeginning, displayedEnd);

            var today = displayedDates.Contains(DateTime.Today)
                ? DateTime.Today
                : (DateTime?)null;

            return selection.Match(
                beginning => getMonthInfoForSingleDate(firstDayOfMonth, selection, displayedDates, today),
                range => getMonthInfoForRange(firstDayOfMonth, selection, month, displayedBeginning, displayedEnd, displayedDates, today));
        }

        private static DateRangePickerMonthInfo getMonthInfoForRange(
            DateTime firstDayOfMonth,
            Either<DateTime, DateRange> selection,
            DateRange month,
            DateTime displayedBeginning,
            DateTime displayedEnd,
            DateRange displayedDates,
            DateTime? today)
        {
            var selectedRange = selection.Right;

            if (!month.OverlapsWith(selectedRange))
                return new DateRangePickerMonthInfo(firstDayOfMonth, displayedDates);

            var displayedSelectionDateBeginning = selectedRange.Beginning > displayedBeginning
                ? selectedRange.Beginning
                : displayedBeginning;

            var displayedSelectionDateEnd = selectedRange.End < displayedEnd
                ? selectedRange.End
                : displayedEnd;

            var displaySelection = new DateRange(displayedSelectionDateBeginning, displayedSelectionDateEnd);

            var isSelectionBeginningBoundary = selectedRange.Beginning == displayedSelectionDateBeginning;
            var isSelectionEndBoundary = selectedRange.End == displayedSelectionDateEnd;

            return new DateRangePickerMonthInfo(
                firstDayOfMonth,
                displayedDates,
                today,
                displaySelection,
                isSelectionBeginningBoundary,
                isSelectionEndBoundary);
        }

        private static DateRangePickerMonthInfo getMonthInfoForSingleDate(
            DateTime firstDayOfMonth,
            Either<DateTime, DateRange> selection,
            DateRange displayedDates,
            DateTime? today)
        {
            var selectedBeginning = selection.Left;
            var isSelectionDisplayed = displayedDates.Contains(selectedBeginning);
            var selectedDateRange = isSelectionDisplayed
                ? new DateRange(selectedBeginning, selectedBeginning)
                : (DateRange?)null;

            return new DateRangePickerMonthInfo(
                  firstDayOfMonth,
                  displayedDates,
                  today,
                  selectedDateRange,
                  isSelectionDisplayed, isSelectionDisplayed);
        }
    }
}
