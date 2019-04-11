using System;
using Toggl.Shared.Models.Reports;

namespace Toggl.Networking.ApiClients
{
    public interface IProjectsSummaryApi
    {
        IObservable<IProjectsSummary> GetByWorkspace(long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate);
    }
}
