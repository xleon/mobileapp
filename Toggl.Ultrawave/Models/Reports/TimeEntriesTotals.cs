using System;
using Toggl.Multivac;
using Toggl.Multivac.Models.Reports;

namespace Toggl.Ultrawave.Models.Reports
{
    [Preserve(AllMembers = true)]
    public sealed class TimeEntriesTotals : ITimeEntriesTotals
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Resolution Resolution { get; set; }

        public ITimeEntriesTotalsGroup[] Groups { get; set; }
    }
}
