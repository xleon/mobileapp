using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Foundation.DTOs;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface ITimeEntriesSource
    {
        IObservable<IDatabaseTimeEntry> CurrentlyRunningTimeEntry { get; }

        IObservable<IDatabaseTimeEntry> TimeEntryCreated { get; }

        IObservable<IDatabaseTimeEntry> TimeEntryUpdated { get; }

        IObservable<bool> IsEmpty { get; }

        IObservable<IEnumerable<IDatabaseTimeEntry>> GetAll();

        IObservable<IEnumerable<IDatabaseTimeEntry>> GetAll(Func<IDatabaseTimeEntry, bool> predicate);

        IObservable<IDatabaseTimeEntry> GetById(long id);

        IObservable<IDatabaseTimeEntry> Start(StartTimeEntryDTO dto);
         IObservable<IDatabaseTimeEntry> Stop(DateTimeOffset stopTime);

        IObservable<Unit> Delete(long id);

        IObservable<IDatabaseTimeEntry> Update(EditTimeEntryDto dto);
    }
}
