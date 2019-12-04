﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.DateRangePicker;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportsViewModel : ViewModel
    {
        private long? selectedWorkspaceId;
        private Either<ReportPeriod, DateRange> selection;

        private readonly IInteractorFactory interactorFactory;
        private readonly ITimeService timeService;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly ITogglDataSource dataSource;

        public IObservable<IImmutableList<IReportElement>> Elements { get; set; }
        public IObservable<bool> HasMultipleWorkspaces { get; set; }
        public IObservable<string> CurrentWorkspaceName { get; private set; }

        public IObservable<string> FormattedTimeRange { get; set; }

        public OutputAction<IThreadSafeWorkspace> SelectWorkspace { get; private set; }
        public OutputAction<DateTimeOffsetRange> SelectTimeRange { get; private set; }

        public ReportsViewModel(
            ITogglDataSource dataSource,
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            ITimeService timeService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;
            this.timeService = timeService;

            HasMultipleWorkspaces = interactorFactory.ObserveAllWorkspaces().Execute()
                .Select(workspaces => workspaces.Where(w => !w.IsInaccessible))
                .Select(w => w.Count() > 1)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            SelectWorkspace = rxActionFactory.FromAsync(selectWorkspace);
            SelectTimeRange = rxActionFactory.FromAsync(selectTimeRange);
        }

        public override async Task Initialize()
        {
            var workspaceSelector = interactorFactory.GetDefaultWorkspace().Execute()
                .Concat(SelectWorkspace.Elements.WhereNotNull());

            var beginningOfWeek = (await interactorFactory.GetCurrentUser().Execute()).BeginningOfWeek;
            var initialSelection = defaultTimeRange(beginningOfWeek);

            var timeRangeSelector = SelectTimeRange.Elements
                .StartWith(initialSelection);

            CurrentWorkspaceName = workspaceSelector
                .Select(ws => ws.Name)
                .StartWith("")
                .DistinctUntilChanged()
                .AsDriver("", schedulerProvider);

            Elements = Observable
                .CombineLatest(workspaceSelector, timeRangeSelector, ReportFilter.Create)
                .SelectMany(reportElements)
                .AsDriver(ImmutableList<IReportElement>.Empty, schedulerProvider);

            var dateFormatObservable = dataSource.Preferences
                .Current
                .Select(preferences => preferences.DateFormat);

            FormattedTimeRange = Observable.Merge(Observable.Return(initialSelection), SelectTimeRange.Elements)
                .CombineLatest(dateFormatObservable, resultSelector: formattedTimeRange)
                .DistinctUntilChanged()
                .AsDriver("", schedulerProvider);

            selectedWorkspaceId = (await interactorFactory.GetDefaultWorkspace().Execute())?.Id;

            selection = Either<ReportPeriod, DateRange>.WithLeft(ReportPeriod.ThisWeek);
        }

        private async Task<IThreadSafeWorkspace> selectWorkspace()
        {
            var allWorkspaces = await interactorFactory.GetAllWorkspaces().Execute();

            var accessibleWorkspaces = allWorkspaces
                .Where(ws => !ws.IsInaccessible)
                .Select(ws => new SelectOption<IThreadSafeWorkspace>(ws, ws.Name))
                .ToImmutableList();

            var currentWorkspaceIndex = accessibleWorkspaces.IndexOf(w => w.Item.Id == selectedWorkspaceId);

            var workspace = await View.Select(Resources.SelectWorkspace, accessibleWorkspaces, currentWorkspaceIndex);

            if (workspace == null || workspace.Id == selectedWorkspaceId)
                return null;

            selectedWorkspaceId = workspace.Id;

            return workspace;
        }

        private async Task<DateTimeOffsetRange> selectTimeRange()
        {
            var selectedTimeRange = await Navigate<DateRangePickerViewModel, Either<ReportPeriod, DateRange>, DateRange>(selection);

            selection = Either<ReportPeriod, DateRange>.WithRight(selectedTimeRange);

            return selectedTimeRange.ToLocalInstantaneousTimeRange();
        }

        private ImmutableList<IReportElement> createLoadingStateReportElements()
            => elements(
                ReportSummaryElement.LoadingState,
                ReportBarChartElement.LoadingState,
                ReportDonutChartDonutElement.LoadingState);

        private IObservable<ImmutableList<IReportElement>> reportElements(ReportFilter filter)
            => reportElementsProcess(filter)
            .ToObservable()
            .StartWith(createLoadingStateReportElements());

        private string formattedTimeRange(DateTimeOffsetRange range, DateFormat dateFormat)
        {
            var startDateText = range.Minimum.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
            var endDateText = range.Maximum.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
            return $"{startDateText} - {endDateText}";
        }

        private async Task<ImmutableList<IReportElement>> reportElementsProcess(ReportFilter filter)
        {
            try
            {
                var user = await interactorFactory.GetCurrentUser().Execute();

                var reportsTotal = await interactorFactory
                    .GetReportsTotals(user.Id, filter.Workspace.Id, filter.TimeRange)
                    .Execute();

                var summaryData = await interactorFactory
                    .GetProjectSummary(filter.Workspace.Id, filter.TimeRange.Minimum, filter.TimeRange.Maximum)
                    .Execute();

                var preferences = await interactorFactory
                    .GetPreferences()
                    .Execute()
                    .FirstAsync();

                var durationFormat = preferences.DurationFormat;
                var dateFormat = preferences.DateFormat;

                if (summaryData.Segments.None())
                    return elements(new ReportNoDataElement());

                return elements(
                    new ReportWorkspaceNameElement(filter.Workspace.Name),
                    new ReportSummaryElement(summaryData, durationFormat),
                    new ReportProjectsBarChartElement(reportsTotal, dateFormat),
                    new ReportProjectsDonutChartElement(summaryData, durationFormat));
            }
            catch (Exception ex)
            {
                return elements(new ReportErrorElement(ex));
            }
        }

        private ImmutableList<IReportElement> elements(params IReportElement[] elements)
            => elements.Flatten();

        private DateTimeOffsetRange defaultTimeRange(BeginningOfWeek beginningOfWeek)
        {
            var today = timeService.CurrentDateTime.ToLocalTime().Date;
            var beginning = today.BeginningOfWeek(beginningOfWeek);
            var end = beginning.AddDays(6);
            return new DateRange(beginning, end).ToLocalInstantaneousTimeRange();
        }
    }
}
