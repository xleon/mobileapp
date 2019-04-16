using System;
using Toggl.Shared.Models.Reports;

namespace Toggl.Networking.ApiClients.Interfaces
{
    public interface ITimeEntriesReportsApi
    {
        IObservable<ITimeEntriesTotals> GetTotals(
            long userId, long workspaceId, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
