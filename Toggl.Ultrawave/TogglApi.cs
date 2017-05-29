using System.Net.Http;
using ModernHttpClient;
using Toggl.Multivac;
using Toggl.Ultrawave.ApiClients;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using static System.Net.DecompressionMethods;

namespace Toggl.Ultrawave
{
    public sealed class TogglApi : ITogglApi
    {
        public TogglApi(ApiEnvironment apiEnvironment, Credentials credentials,
            HttpClientHandler handler = null)
        {
            Ensure.ArgumentIsNotNull(credentials, nameof(credentials));

            var httpHandler = handler ?? new NativeMessageHandler { AutomaticDecompression = GZip | Deflate };
            var httpClient = new HttpClient(httpHandler);

            var serializer = new JsonSerializer();
            var apiClient = new ApiClient(httpClient);
            var endpoints = new Endpoints(apiEnvironment);

            Tags = new TagsApi();
            User = new UserApi(endpoints.User, apiClient, serializer, credentials);
            Tasks = new TasksApi();
            Status = new StatusApi(apiClient);
            Clients = new ClientsApi(endpoints.Clients, apiClient, serializer, credentials);
            Projects = new ProjectsApi();
            Workspaces = new WorkspacesApi(endpoints.Workspaces, apiClient, serializer, credentials);
            TimeEntries = new TimeEntriesApi(endpoints.TimeEntries, apiClient, serializer, credentials);
        }

        public ITagsApi Tags { get; }
        public IUserApi User { get; }
        public ITasksApi Tasks { get; }
        public IStatusApi Status { get; }
        public IClientsApi Clients { get; }
        public IProjectsApi Projects { get; }
        public IWorkspacesApi Workspaces { get; }
        public ITimeEntriesApi TimeEntries { get; }
    }
}
