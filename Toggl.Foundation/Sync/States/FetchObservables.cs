using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.States
{
    internal sealed class FetchObservables
    {
        public ISinceParameters SinceParameters { get; }
        public IObservable<List<IWorkspace>> Workspaces { get; }
        public IObservable<List<IClient>> Clients { get; }
        public IObservable<List<IProject>> Projects { get; }
        public IObservable<List<ITimeEntry>> TimeEntries { get; }

        public FetchObservables(ISinceParameters sinceParameters,
            IObservable<List<IWorkspace>> workspaces, IObservable<List<IClient>> clients,
            IObservable<List<IProject>> projects, IObservable<List<ITimeEntry>> timeEntries
        )
        {
            SinceParameters = sinceParameters;
            Workspaces = workspaces;
            Clients = clients;
            Projects = projects;
            TimeEntries = timeEntries;
        }
    }
}
