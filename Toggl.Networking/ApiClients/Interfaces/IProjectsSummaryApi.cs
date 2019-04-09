using System;
using Toggl.Shared.Models.Reports;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IProjectsSummaryApi
    {
        IObservable<IProjectsSummary> GetByWorkspace(long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate);
    }
}
