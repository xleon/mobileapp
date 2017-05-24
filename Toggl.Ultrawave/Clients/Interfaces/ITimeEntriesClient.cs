using System;
using System.Collections.Generic;

namespace Toggl.Ultrawave.Clients
{
    public interface ITimeEntriesClient
    {
        IObservable<List<TimeEntry>> GetAll();
        IObservable<TimeEntry> Create(TimeEntry timeEntry);
    }
}
