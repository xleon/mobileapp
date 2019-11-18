using System;
using Toggl.Core.Interactors.Reports;
using Toggl.Core.Reports;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.Interactors
{
    public partial class InteractorFactory
    {
        public IInteractor<IObservable<ITimeEntriesTotals>> GetReportsTotals(
            long userId, long workspaceId, DateTimeOffsetRange timeRange)
            => new GetTotalsInteractor(api.TimeEntriesReports, userId, workspaceId, timeRange);

        public IInteractor<IObservable<ProjectSummaryReport>> GetProjectSummary(
            long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate)
            => new GetProjectSummaryInteractor(api, database, analyticsService, reportsMemoryCache, workspaceId, startDate, endDate)
                .TrackException<Exception, ProjectSummaryReport>("GetProjectSummary");
    }
}
