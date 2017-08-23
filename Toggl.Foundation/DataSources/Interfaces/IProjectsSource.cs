using System;
using System.Collections.Generic;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IProjectsSource
    {
        IObservable<IDatabaseProject> GetById(long id);
        IObservable<IEnumerable<IDatabaseProject>> GetAll();
        IObservable<IEnumerable<IDatabaseProject>> GetAll(Func<IDatabaseProject, bool> predicate);
    }
}