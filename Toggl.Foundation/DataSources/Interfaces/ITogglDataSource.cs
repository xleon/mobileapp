using System;
using System.Reactive;

namespace Toggl.Foundation.DataSources
{
    public interface ITogglDataSource
    {
        ITagsSource Tags { get; }
        IUserSource User { get; }
        ITasksSource Tasks { get; }
        IClientsSource Clients { get; }
        IProjectsSource Projects { get; }
        IWorkspacesSource Workspaces { get; }
        ITimeEntriesSource TimeEntries { get; }

        IObservable<Unit> Logout();
    }
}
