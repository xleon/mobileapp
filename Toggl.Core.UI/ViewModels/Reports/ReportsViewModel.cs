using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    using WorkspaceSelectOptions = ImmutableList<SelectOption<IThreadSafeWorkspace>>;

    public sealed class ReportsViewModel : ViewModel
    {
        private long selectedWorkspaceId;
        private DateTimeOffsetRange selectedTimeRange;

        private IInteractorFactory interactorFactory;

        public IObservable<IEnumerable<IReportElement>> Elements { get; set; }
        public IObservable<bool> HasMultipleWorkspaces { get; set; }

        public OutputAction<IThreadSafeWorkspace> SelectWorkspace { get; private set; }
        public OutputAction<DateTimeOffsetRange> SelectTimeRange { get; private set; }

        public ReportsViewModel(
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
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

            Elements = Observable.CombineLatest(
                SelectWorkspace.Elements.WhereNotNull(),
                SelectTimeRange.Elements,
                reportElements)
                .StartWith(ImmutableList<IReportElement>.Empty)
                .AsDriver(ImmutableList<IReportElement>.Empty, schedulerProvider);
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
            selectedTimeRange = await Task.FromResult(getDummyResult());

            return selectedTimeRange;
        }

        [Obsolete("Remove this in favor of data from the Reports Calendar results")]
        private DateTimeOffsetRange getDummyResult()
            => new DateTimeOffsetRange(
                new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));

        private IEnumerable<IReportElement> reportElements(IThreadSafeWorkspace workspace, DateTimeOffsetRange timeRange)
        {
            // TODO: Refetch and update Elements
            return ImmutableList<IReportElement>.Empty;
        }
    }
}
