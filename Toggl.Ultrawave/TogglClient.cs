using Toggl.Ultrawave.Clients;
using static Toggl.Ultrawave.Helpers.ApiUrls;

namespace Toggl.Ultrawave
{
    public sealed class TogglClient : ITogglClient
    {
        private readonly string baseUrl;

        internal TogglClient(ApiEnvironment apiEnvironment)
        {
            baseUrl = apiEnvironment == ApiEnvironment.Production ? ProductionBaseUrl : StagingBaseUrl;

            Tags = new TagsClient();
            User = new UserClient();
            Tasks = new TasksClient();
            Clients = new ClientsClient();
            Projects = new ProjectsClient();
            Workspaces = new WorkspacesClient();
            TimeEntries = new TimeEntriesClient();
        }

        public ITagsClient Tags { get; }
        public IUserClient User { get; }
        public ITasksClient Tasks { get; }
        public IClientsClient Clients { get; }
        public IProjectsClient Projects { get; }
        public IWorkspacesClient Workspaces { get; }
        public ITimeEntriesClient TimeEntries { get; }

        public static ITogglClient WithBaseUrl(ApiEnvironment apiEnvironment) => new TogglClient(apiEnvironment);
    }
}
