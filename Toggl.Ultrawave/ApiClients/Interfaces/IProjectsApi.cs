using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IProjectsApi
    {
        IObservable<List<Project>> GetAll();
        IObservable<Project> Create(Project project);
    }
}
