using System;
using Toggl.Ultrawave.Helpers;

namespace Toggl.Ultrawave.Network
{
    internal sealed class Endpoints
    {
        private readonly Uri baseUrl;

        public UserEndpoints User => new UserEndpoints(baseUrl);
        public WorkspaceEndpoints Workspaces => new WorkspaceEndpoints(baseUrl);
        public TagEndpoints Tags => new TagEndpoints(baseUrl);

        public Endpoints(ApiEnvironment environment)
        {
            baseUrl = ApiUrls.ForEnvironment(environment);
        }
    }
}