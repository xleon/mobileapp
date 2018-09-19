using System;
namespace Toggl.Multivac.Models.Reports
{
    public interface ITimeEntriesTotals
    {
        DateTimeOffset StartDate { get; }

        DateTimeOffset EndDate { get; }

        Resolution Resolution { get; }

        ITimeEntriesTotalsGroup[] Groups { get; }
    }
}
