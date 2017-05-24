using System.Net.Http;
using ModernHttpClient;
using Toggl.Multivac;
using Toggl.Ultrawave.Clients;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using static System.Net.DecompressionMethods;

namespace Toggl.Ultrawave
{
    public sealed class TogglClient : ITogglClient
    {
        private readonly Endpoints endpoints;

        public TogglClient(ApiEnvironment apiEnvironment, Credentials credentials,
            HttpClientHandler handler = null)
        {
            Ensure.ArgumentIsNotNull(credentials, nameof(credentials));

            var httpHandler = handler ?? new NativeMessageHandler { AutomaticDecompression = GZip | Deflate };
            var httpClient = new HttpClient(httpHandler);

            var serializer = new JsonSerializer();
            var apiClient = new ApiClient(httpClient);
            endpoints = new Endpoints(apiEnvironment);

            Tags = new TagsClient();
            User = new UserClient(endpoints.User, apiClient, serializer, credentials);
            Tasks = new TasksClient();
            Status = new StatusClient(apiClient);
            Clients = new ClientsClient(endpoints.Clients, apiClient, serializer, credentials);
            Projects = new ProjectsClient();
            Workspaces = new WorkspacesClient(endpoints.Workspaces, apiClient, serializer, credentials);
            TimeEntries = new TimeEntriesClient(endpoints.TimeEntries, apiClient, serializer, credentials);
        }

        public ITagsClient Tags { get; }
        public IUserClient User { get; }
        public ITasksClient Tasks { get; }
        public IStatusClient Status { get; }
        public IClientsClient Clients { get; }
        public IProjectsClient Projects { get; }
        public IWorkspacesClient Workspaces { get; }
        public ITimeEntriesClient TimeEntries { get; }
    }
}
