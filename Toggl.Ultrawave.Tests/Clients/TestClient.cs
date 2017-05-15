using System;
using System.Collections.Generic;
using Toggl.Multivac;
using Toggl.Ultrawave.Clients;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Tests.Clients
{
    internal sealed class TestClient : BaseClient
    {
        private readonly Endpoint endpoint;

        public TestClient(Endpoint endpoint, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials)
        {
            Ensure.ArgumentIsNotNull(endpoint, nameof(endpoint));

            this.endpoint = endpoint;
        }

        public IObservable<T> TestCreateObservable<T>(Endpoint endpoint, IEnumerable<HttpHeader> headers, string body = "")
            => CreateObservable<T>(endpoint, headers, body);

        public IObservable<string> Get()
        {
            var observable = CreateObservable<string>(endpoint, AuthHeader);
            return observable;
        }
    }
}
