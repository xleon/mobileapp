using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IProjectsApi
    {
        IObservable<List<IProject>> GetAll();
        IObservable<List<IProject>> GetAllSince(DateTimeOffset threshold);
        IObservable<IProject> Create(IProject project);
    }
}
