using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.DataSources
{
    public interface ITimeEntriesSource
    {
        IObservable<IEnumerable<ITimeEntry>> GetAll();
        IObservable<Unit> Delete(int id);
    }
}
