using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportsViewModel : ViewModel
    {
        private long? selectedWorkspaceId;
        private DateTimeOffsetRange selectedTimeRange;

        private readonly IInteractorFactory interactorFactory;

        public IObservable<IImmutableList<IReportElement>> Elements { get; set; }
        public IObservable<bool> HasMultipleWorkspaces { get; set; }

        public IObservable<string> FormattedTimeRange { get; set; }

        public OutputAction<IThreadSafeWorkspace> SelectWorkspace { get; private set; }
        public OutputAction<DateTimeOffsetRange> SelectTimeRange { get; private set; }

        public ReportsViewModel(
            ITogglDataSource dataSource,
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.interactorFactory = interactorFactory;

            HasMultipleWorkspaces = interactorFactory.ObserveAllWorkspaces().Execute()
                .Select(workspaces => workspaces.Where(w => !w.IsInaccessible))
                .Select(w => w.Count() > 1)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            SelectWorkspace = rxActionFactory.FromAsync(selectWorkspace);
            SelectTimeRange = rxActionFactory.FromAsync(selectTimeRange);

            var workspaceSelector = interactorFactory.GetDefaultWorkspace().Execute()
                .Concat(SelectWorkspace.Elements.WhereNotNull());

            // TODO: Get default range (current week) instead of this hardcoded value
            var defaultTimeRange = new DateTimeOffsetRange(DateTimeOffset.Now - TimeSpan.FromDays(7), DateTimeOffset.Now);

            var timeRangeSelector = SelectTimeRange.Elements.StartWith(defaultTimeRange);

            Elements = Observable
                .CombineLatest(workspaceSelector, timeRangeSelector, ReportFilter.Create)
                .SelectMany(reportElements)
                .AsDriver(ImmutableList<IReportElement>.Empty, schedulerProvider);

            var dateFormatObservable = dataSource.Preferences
                .Current
                .Select(preferences => preferences.DateFormat);

            FormattedTimeRange = Observable.Merge(Observable.Return(defaultTimeRange), SelectTimeRange.Elements)
                .CombineLatest(dateFormatObservable, resultSelector: formattedTimeRange)
                .DistinctUntilChanged()
                .AsDriver("", schedulerProvider);
        }

        public override async Task Initialize()
        {
            var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
            selectedWorkspaceId = defaultWorkspace?.Id;
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
            // TODO: Navigate to reports calendar fragment and replace this Task with it
            selectedTimeRange = await Task.FromResult(getDummyTimeRange());

            return selectedTimeRange;
        }

        [Obsolete("Remove this in favor of data from the Reports Calendar results")]
        private DateTimeOffsetRange getDummyTimeRange()
            => new DateTimeOffsetRange(
                new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));

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

                var durationFormat = await interactorFactory
                    .GetPreferences()
                    .Execute()
                    .FirstAsync()
                    .Select(preferences => preferences.DurationFormat);

                if (summaryData.Segments.None())
                    return elements(new ReportNoDataElement());

                return elements(
                    new ReportWorkspaceNameElement(filter.Workspace.Name),
                    new ReportSummaryElement(summaryData, durationFormat),
                    new ReportProjectsBarChartElement(reportsTotal, durationFormat),
                    new ReportProjectsDonutChartElement(summaryData, durationFormat));
            }
            catch (Exception ex)
            {
                return elements(new ReportErrorElement(ex));
            }
        }

        private ImmutableList<IReportElement> elements(params IReportElement[] elements)
            => elements.Flatten();
    }
}
