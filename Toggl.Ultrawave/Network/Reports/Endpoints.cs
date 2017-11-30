using System;
using Toggl.Ultrawave.Helpers;

namespace Toggl.Ultrawave.Network.Reports
{
    internal sealed class Endpoints
    {
        private readonly Uri baseUrl;

        public ProjectsSummaryEndpoints ProjectsSummaries => new ProjectsSummaryEndpoints(baseUrl);

        public Endpoints(ApiEnvironment environment)
        {
            baseUrl = ReportsUrls.ForEnvironment(environment);
        }
    }
}
