using Toggl.Ultrawave.Clients;

namespace Toggl.Ultrawave
{
    public interface ITogglClient
    {
        ITagsClient Tags { get; }
        IUserClient User { get; }
        ITasksClient Tasks { get; }
        IClientsClient Clients { get; }
        IProjectsClient Projects { get; }
        IWorkspacesClient Workspaces { get; }
        ITimeEntriesClient TimeEntries { get; }
    }
}
