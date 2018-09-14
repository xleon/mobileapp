using System;
using Toggl.Multivac.Models.Reports;

namespace Toggl.Ultrawave.ApiClients.Interfaces
{
    public interface ITimeEntriesReportsApi
    {
        IObservable<ITimeEntriesTotals> GetTotals(long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate);
    }
}
