using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.DataSources
{
    public interface IProjectsSource
    {
        IObservable<IEnumerable<IProject>> GetAll();
        IObservable<IProject> GetById(int id);
    }
}