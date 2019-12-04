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

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public sealed partial class DateRangePickerViewModel : ViewModel<Either<ReportPeriod, DateRange>, DateRange>
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
        private readonly ITimeService timeService;

        private BeginningOfWeek beginningOfWeek;
        private BehaviorSubject<SelectionState> selectionState;

        private readonly List<DatePickerShortcut> shortcuts = new List<DatePickerShortcut>();

        public ImmutableList<string> WeekDaysLabels { get; private set; }

        public IObservable<ImmutableList<DateRangePickerMonthInfo>> Months { get; private set; }
        public IObservable<ReportPeriod> SelectedShortcut { get; private set; }

        public RxAction<ReportPeriod, SelectionState> SetReportPeriod { get; }
        public RxAction<DateTime, SelectionState> SelectDate { get; }

        public DateRangePickerViewModel(
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            IRxActionFactory rxActionFactory,
            ISchedulerProvider schedulerProvider,
            ITimeService timeService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;
            this.timeService = timeService;

            SelectDate = rxActionFactory.FromFunction<DateTime, SelectionState>(selectDate);
            SetReportPeriod = rxActionFactory.FromFunction<ReportPeriod, SelectionState>(setReportPeriod);
        }

        public override async Task Initialize(Either<ReportPeriod, DateRange> initialSelection)
        {
            beginningOfWeek = await interactorFactory.GetCurrentUser().Execute()
                .Select(u => u.BeginningOfWeek)
                .FirstAsync();

            WeekDaysLabels = getWeekDaysLabels(beginningOfWeek);

            shortcuts.AddRange(createShortcuts(beginningOfWeek, timeService.CurrentDateTime));

            var initialRange = unwrapInitialRange(initialSelection);
            selectionState = new BehaviorSubject<SelectionState>(SelectionState.WithRight(initialRange));

            var selectionChange = Observable.Merge(SelectDate.Elements, SetReportPeriod.Elements);

            Months = selectionChange
                .Select(getMonthsData)
                .AsDriver(ImmutableList<DateRangePickerMonthInfo>.Empty, schedulerProvider);

            SelectedShortcut = selectionChange
                .Select(reportPeriodFromSelection)
                .AsDriver(ReportPeriod.Unknown, schedulerProvider);
        }

        private DateRange unwrapInitialRange(Either<ReportPeriod, DateRange> initialSelection)
            => initialSelection.Match(
                period => shortcuts.First(s => s.Period == period).DateRange,
                range => range);

        private SelectionState selectDate(DateTime date)
            => selectionState.Value.Match(
                beginning => SelectionState.WithRight(new DateRange(beginning, date)),
                range => SelectionState.WithLeft(date));

        private SelectionState setReportPeriod(ReportPeriod date)
            => SelectionState.WithRight(shortcuts.First(s => s.Period == date).DateRange);

        private ReportPeriod reportPeriodFromSelection(SelectionState state)
            => state.Match(
                beginning => ReportPeriod.Unknown,
                range => shortcuts.FirstOrDefault(s => s.MatchesDateRange(range))?.Period ?? ReportPeriod.Unknown);

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
