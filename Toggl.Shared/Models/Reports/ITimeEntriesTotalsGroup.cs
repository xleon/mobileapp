using System;
namespace Toggl.Multivac.Models.Reports
{
    public interface ITimeEntriesTotalsGroup
    {
        TimeSpan Total { get; }

        TimeSpan Billable { get; }
    }
}
