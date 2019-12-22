using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Models;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Models;
using SelectionState = Toggl.Shared.Either<System.DateTime, Toggl.Shared.DateRange>;
using DateRangeSelectionResult = Toggl.Core.UI.ViewModels.DateRangePicker.DateRangePickerViewModel.DateRangeSelectionResult;
using Toggl.Core.UI.Services;
using static Toggl.Core.Models.DateRangePeriod;
using static Toggl.Core.Analytics.DateRangeSelectionSource;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public sealed partial class DateRangePickerViewModel : ViewModel<Either<DateRangePeriod, DateRange>, DateRangeSelectionResult>
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
        private readonly IDateRangeShortcutsService dateRangeShortcutsService;

        private BeginningOfWeek beginningOfWeek;
        private SelectionState selectionState;

        private DateRange? selectedRange;
        private DateRangeSelectionSource dateRangeSelectionSource;

        public ImmutableList<string> WeekDaysLabels { get; private set; }

        public IObservable<ImmutableList<DateRangePickerMonthInfo>> Months { get; private set; }
        public IObservable<ImmutableList<Shortcut>> Shortcuts { get; private set; }

        public IObservable<DateTime?> LastSelectedDate { get; private set; }

        public RxAction<DateRangePeriod, SelectionState> SetDateRangePeriod { get; }
        public RxAction<DateTime, SelectionState> SelectDate { get; }
        public ViewAction Accept { get; }
        public ViewAction Cancel { get; }

        public DateRangePickerViewModel(
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            IRxActionFactory rxActionFactory,
            ISchedulerProvider schedulerProvider,
            IDateRangeShortcutsService dateRangeShortcutsService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(dateRangeShortcutsService, nameof(dateRangeShortcutsService));

            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;
            this.dateRangeShortcutsService = dateRangeShortcutsService;

            SelectDate = rxActionFactory.FromFunction<DateTime, SelectionState>(selectDate);
            SetDateRangePeriod = rxActionFactory.FromFunction<DateRangePeriod, SelectionState>(setDateRangePeriod);

            Accept = rxActionFactory.FromAction(accept);
            Cancel = rxActionFactory.FromAction(cancel);
        }

        public override async Task Initialize(Either<DateRangePeriod, DateRange> initialSelection)
        {
            beginningOfWeek = await interactorFactory.GetCurrentUser().Execute()
                .Select(u => u.BeginningOfWeek)
                .FirstAsync();

            WeekDaysLabels = getWeekDaysLabels(beginningOfWeek);

            var initialRange = unwrapInitialRange(initialSelection);
            selectionState = SelectionState.WithRight(initialRange);

            dateRangeSelectionSource = Initial;

            var initialSelectionObservable = Observable.Return(selectionState);

            var selectionChange = Observable.Merge(initialSelectionObservable, SelectDate.Elements, SetDateRangePeriod.Elements)
                .Do(saveSelectedRange);

            Months = selectionChange
                .Select(getMonthsData)
                .AsDriver(ImmutableList<DateRangePickerMonthInfo>.Empty, schedulerProvider);

            Shortcuts = selectionChange
                .Select(dateRangePeriodFromSelection)
                .Select(shortcutsFromSelectedPeriod)
                .AsDriver(shortcutsFromSelectedPeriod(Unknown), schedulerProvider);

            LastSelectedDate = selectionChange
                .Select(s => s.Match(date => (DateTime?)null, range => range.End))
                .AsDriver(null, schedulerProvider);
        }

        private DateRange unwrapInitialRange(Either<DateRangePeriod, DateRange> initialSelection)
            => initialSelection.Match(
                period => dateRangeShortcutsService.GetShortcutFrom(period).DateRange,
                range => range);

        private void saveSelectedRange(SelectionState selection)
        {
            selectedRange = selection.Match(partialRange => (DateRange?)null, range => range);
        }



        private SelectionState selectDate(DateTime date)
        {
            selectionState = selectionState.Match(
                beginning => SelectionState.WithRight(new DateRange(beginning, date)),
                range => SelectionState.WithLeft(date));
            dateRangeSelectionSource = DateRangeSelectionSource.Calendar;

            return selectionState;
        }

        private void cancel()
        {
            Close(null);
        }

        private void accept()
        {
            Close(new DateRangeSelectionResult(selectedRange, dateRangeSelectionSource));
        }

        private SelectionState setDateRangePeriod(DateRangePeriod period)
        {
            var selectedShortcut = dateRangeShortcutsService.GetShortcutFrom(period);
            dateRangeSelectionSource = getSelectionSource(selectedShortcut);
            return SelectionState.WithRight(selectedShortcut.DateRange);
        }

        private DateRangeSelectionSource getSelectionSource(DateRangeShortcut selectedShortcut)
        {
            if (selectedShortcut == null)
                return DateRangeSelectionSource.Calendar;

            return selectedShortcut.Period switch
            {
                Today => ShortcutToday,
                Yesterday => ShortcutYesterday,
                ThisWeek => ShortcutThisWeek,
                LastWeek => ShortcutLastWeek,
                ThisMonth => ShortcutThisMonth,
                LastMonth => ShortcutLastMonth,
                ThisYear => ShortcutThisYear,
                LastYear => ShortcutLastYear,
                _ => Other
            };
        }

        private DateRangePeriod dateRangePeriodFromSelection(SelectionState state)
            => state.Match(
                beginning => DateRangePeriod.Unknown,
                range => dateRangeShortcutsService.GetShortcutFrom(range)?.Period ?? DateRangePeriod.Unknown);

        private ImmutableList<Shortcut> shortcutsFromSelectedPeriod(DateRangePeriod period)
            => dateRangeShortcutsService
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

        public class DateRangeSelectionResult
        {
            public DateRange? SelectedRange { get; }
            public DateRangeSelectionSource Source { get; }

            public DateRangeSelectionResult(DateRange? selectedRange, DateRangeSelectionSource source)
            {
                SelectedRange = selectedRange;
                Source = source;
            }
        }
    }
}
