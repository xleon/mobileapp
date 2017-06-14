using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.DataSources
{
    public interface ITimeEntriesSource
    {
        IObservable<IEnumerable<ITimeEntry>> GetAll();
    }
}