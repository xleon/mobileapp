using System;
using System.Collections.Generic;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IProjectsSource
    {
        IObservable<IEnumerable<IDatabaseProject>> GetAll();
        IObservable<IDatabaseProject> GetById(long id);
    }
}