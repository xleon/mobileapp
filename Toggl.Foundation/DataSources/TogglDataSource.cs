using System;
using System.Reactive;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;

namespace Toggl.Foundation.DataSources
{
    public sealed class TogglDataSource : ITogglDataSource
    {
        private readonly ITogglDatabase database;

        public TogglDataSource(ITogglDatabase database, ITogglApi api)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(database, nameof(database));

            this.database = database;

            User = new UserDataSource(database.User, api.User);
            Projects = new ProjectsDataSource(database.Projects);
            TimeEntries = new TimeEntriesDataSource(database.IdProvider, database.TimeEntries);
        }

        public IUserSource User { get; }
        public ITagsSource Tags => throw new NotImplementedException();
        public ITasksSource Tasks => throw new NotImplementedException();
        public IClientsSource Clients => throw new NotImplementedException();
        public IProjectsSource Projects { get; }
        public IWorkspacesSource Workspaces => throw new NotImplementedException();
        public ITimeEntriesSource TimeEntries { get; }

        public IObservable<Unit> Logout() => database.Clear();
    }
}
