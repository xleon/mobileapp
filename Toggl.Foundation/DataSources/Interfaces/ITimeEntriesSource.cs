using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.DataSources
{
    public interface ITimeEntriesSource
    {
        IObservable<ITimeEntry> CurrentlyRunningTimeEntry { get; }

        IObservable<IEnumerable<ITimeEntry>> GetAll();

        IObservable<ITimeEntry> Start(DateTimeOffset startTime, string description, bool billable);

        IObservable<Unit> Delete(long id);
    }
}
