using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Models;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using SelectionState = Toggl.Shared.Either<System.DateTime, Toggl.Shared.DateRange>;
using Toggl.Core.UI.Services;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public sealed partial class DateRangePickerViewModel : ViewModel<Either<ReportPeriod, DateRange>, DateRange?>
    {
        private const int daysInWeek = 7;
        private static readonly string[] dayHeaders =
        {
            Resources.SundayInitial,
            Resources.MondayInitial,
            Resources.TuesdayInitial,
            Resources.WednesdayInitial,
            Resources.ThursdayInitial,
            Resources.FridayInitial,
            Resources.SaturdayInitial
        };

        private readonly IInteractorFactory interactorFactory;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly ICalendarShortcutsService calendarShortcutsService;

        private BeginningOfWeek beginningOfWeek;
        private SelectionState selectionState;

        private DateRange? result;

        public ImmutableList<string> WeekDaysLabels { get; private set; }

        public IObservable<ImmutableList<DateRangePickerMonthInfo>> Months { get; private set; }
        public IObservable<ImmutableList<Shortcut>> Shortcuts { get; private set; }

        public IObservable<DateTime?> LastSelectedDate { get; private set; }

        public RxAction<ReportPeriod, SelectionState> SetReportPeriod { get; }
        public RxAction<DateTime, SelectionState> SelectDate { get; }
        public ViewAction Accept { get; }
        public ViewAction Cancel { get; }

        public DateRangePickerViewModel(
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            IRxActionFactory rxActionFactory,
            ISchedulerProvider schedulerProvider,
            ICalendarShortcutsService calendarShortcutsService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(calendarShortcutsService, nameof(calendarShortcutsService));

            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;
            this.calendarShortcutsService = calendarShortcutsService;

            SelectDate = rxActionFactory.FromFunction<DateTime, SelectionState>(selectDate);
            SetReportPeriod = rxActionFactory.FromFunction<ReportPeriod, SelectionState>(setReportPeriod);

            Accept = rxActionFactory.FromAction(accept);
            Cancel = rxActionFactory.FromAction(cancel);
        }

        public override async Task Initialize(Either<ReportPeriod, DateRange> initialSelection)
        {
            beginningOfWeek = await interactorFactory.GetCurrentUser().Execute()
                .Select(u => u.BeginningOfWeek)
                .FirstAsync();

            WeekDaysLabels = getWeekDaysLabels(beginningOfWeek);

            var initialRange = unwrapInitialRange(initialSelection);
            selectionState = SelectionState.WithRight(initialRange);

            var initialSelectionObservable = Observable.Return(selectionState);

            var selectionChange = Observable.Merge(initialSelectionObservable, SelectDate.Elements, SetReportPeriod.Elements)
                .Do(saveResult);

            Months = selectionChange
                .Select(getMonthsData)
                .AsDriver(ImmutableList<DateRangePickerMonthInfo>.Empty, schedulerProvider);

            Shortcuts = selectionChange
                .Select(reportPeriodFromSelection)
                .Select(shortcutsFromSelectedPeriod)
                .AsDriver(shortcutsFromSelectedPeriod(ReportPeriod.Unknown), schedulerProvider);

            LastSelectedDate = selectionChange
                .Select(s => s.Match(date => (DateTime?)null, range => range.End))
                .AsDriver(null, schedulerProvider);
        }

        private DateRange unwrapInitialRange(Either<ReportPeriod, DateRange> initialSelection)
            => initialSelection.Match(
                period => calendarShortcutsService.GetShortcutFrom(period).DateRange,
                range => range);

        private void saveResult(SelectionState selection)
        {
            result = selection.Match(partialRange => (DateRange?)null, range => range);
        }

        private SelectionState selectDate(DateTime date)
        {
            selectionState = selectionState.Match(
                beginning => SelectionState.WithRight(new DateRange(beginning, date)),
                range => SelectionState.WithLeft(date));

            return selectionState;
        }

        private void cancel()
        {
            Close(null);
        }

        private void accept()
        {
            Close(result);
        }

        private SelectionState setReportPeriod(ReportPeriod period)
            => SelectionState.WithRight(calendarShortcutsService.GetShortcutFrom(period).DateRange);

        private ReportPeriod reportPeriodFromSelection(SelectionState state)
            => state.Match(
                beginning => ReportPeriod.Unknown,
                range => calendarShortcutsService.GetShortcutFrom(range)?.Period ?? ReportPeriod.Unknown);

        private ImmutableList<Shortcut> shortcutsFromSelectedPeriod(ReportPeriod period)
            => calendarShortcutsService
                .Shortcuts
                .Select(shortcut => Shortcut.From(shortcut, shortcut.Period == period))
                .ToImmutableList();

        private ImmutableList<DateRangePickerMonthInfo> getMonthsData(SelectionState selectionState)
        {
            const int monthsCount = 2 * 12 + 1; // Two years + include the boundary month.

            var thisMonth = DateTime.Today.FirstDayOfSameMonth();
            var twoYearsAgo = thisMonth.AddYears(-2);

            return Enumerable.Range(0, monthsCount)
                .Select(month => twoYearsAgo.AddMonths(month))
                .Select(month => DateRangePickerMonthInfo.Create(month, selectionState, beginningOfWeek))
                .ToImmutableList();
        }

        private static ImmutableList<string> getWeekDaysLabels(BeginningOfWeek beginningOfWeek)
            => Enumerable.Range((int)beginningOfWeek, 7)
            .Select(index => dayHeaders[index % daysInWeek])
            .ToImmutableList();
    }
}
