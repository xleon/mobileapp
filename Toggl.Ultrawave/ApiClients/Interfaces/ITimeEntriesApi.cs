using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface ITimeEntriesApi
    {
        IObservable<List<TimeEntry>> GetAll();
        IObservable<TimeEntry> Create(TimeEntry timeEntry);
    }
}
