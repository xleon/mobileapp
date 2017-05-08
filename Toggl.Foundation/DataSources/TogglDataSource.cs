using System;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;

namespace Toggl.Foundation.DataSources
{
    public sealed class TogglDataSource : ITogglDataSource
    {
        private TogglDataSource(ITogglDatabase database, ITogglClient api)
        {
            Ensure.ArgumentIsNotNull(api, nameof(api));
            Ensure.ArgumentIsNotNull(database, nameof(database));
        }

        public ITagsSource Tags => throw new NotImplementedException();
        public IUserSource User => throw new NotImplementedException();
        public ITasksSource Tasks => throw new NotImplementedException();
        public IClientsSource Clients => throw new NotImplementedException();
        public IProjectsSource Projects => throw new NotImplementedException();
		public IWorkspacesSource Workspaces => throw new NotImplementedException();
        public ITimeEntriesSource TimeEntries => throw new NotImplementedException();
    }
}
