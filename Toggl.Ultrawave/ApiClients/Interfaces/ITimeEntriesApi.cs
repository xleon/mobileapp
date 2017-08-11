using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface ITimeEntriesApi
    {
        IObservable<List<ITimeEntry>> GetAll();
        IObservable<ITimeEntry> Create(ITimeEntry timeEntry);
    }
}
