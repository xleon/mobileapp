using System;
using Toggl.Shared.Models.Reports;
namespace Toggl.Ultrawave.Models.Reports
{
    public sealed class TimeEntriesTotalsGroup : ITimeEntriesTotalsGroup
    {
        public TimeSpan Total { get; set; }

        public TimeSpan Billable { get; set; }
    }
}
