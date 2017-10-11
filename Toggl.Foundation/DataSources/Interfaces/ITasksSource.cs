using System;
using System.Collections.Generic;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface ITasksSource
    {
        IObservable<IDatabaseTask> GetById(long id);
        IObservable<IEnumerable<IDatabaseTask>> GetAll();
        IObservable<IEnumerable<IDatabaseTask>> GetAll(Func<IDatabaseTask, bool> predicate);
    }
}