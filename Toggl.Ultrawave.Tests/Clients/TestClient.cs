using Toggl.Multivac;
using Toggl.Ultrawave.Clients;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Tests.Clients
{
    internal sealed class TestClient : BaseClient
    {
        private readonly Endpoint endpoint;

        public TestClient(Endpoint endpoint, IApiClient apiClient, IJsonSerializer serializer)
            : base(apiClient, serializer)
        {
            Ensure.ArgumentIsNotNull(endpoint, nameof(endpoint));

            this.endpoint = endpoint;
        }

        public ICall<string> Get(string username, string password)
        {
            var header = GetAuthHeader(username, password);
            var call = CreateCall<string>(endpoint, header);
            return call;
        }
    }
}
