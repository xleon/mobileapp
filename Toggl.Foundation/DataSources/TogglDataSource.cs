using System;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;

namespace Toggl.Foundation.DataSources
{
    public sealed class TogglDataSource : ITogglDataSource
    {
        public TogglDataSource(ITogglDatabase database, ITogglApi api)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(database, nameof(database));

            User = new UserDataSource(database.User, api.User);
            Projects = new ProjectsDataSource(database.Projects);
            TimeEntries = new TimeEntriesDataSource(database.TimeEntries);
        }

        public IUserSource User { get; }
        public ITagsSource Tags => throw new NotImplementedException();
        public ITasksSource Tasks => throw new NotImplementedException();
        public IClientsSource Clients => throw new NotImplementedException();
        public IProjectsSource Projects { get; }
        public IWorkspacesSource Workspaces => throw new NotImplementedException();
        public ITimeEntriesSource TimeEntries { get; }
    }
}
