using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.PullTimeEntries
{
    public sealed class CreatePlaceholdersStateFactory
    {
        private readonly ITogglDataSource dataSource;
        private readonly IAnalyticsService analyticsService;

        public CreatePlaceholdersStateFactory(
            ITogglDataSource dataSource, IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.dataSource = dataSource;
            this.analyticsService = analyticsService;
        }

        public CreatePlaceholdersState<IThreadSafeProject, IDatabaseProject> ForProjects()
            => new CreatePlaceholdersState<IThreadSafeProject, IDatabaseProject>(
                dataSource.Projects,
                analyticsService.ProjectPlaceholdersCreated,
                timeEntry => timeEntry.ProjectId.HasValue
                    ? new[] { timeEntry.ProjectId.Value }
                    : new long[0],
                Project.CreatePlaceholder);
    }
}
