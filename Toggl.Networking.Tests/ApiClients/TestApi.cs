using System;
using System.Collections.Generic;
using Toggl.Shared;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;

namespace Toggl.Networking.Tests.ApiClients
{
    internal sealed class TestApi : BaseApi
    {
        private readonly Endpoint endpoint;

        public TestApi(Endpoint endpoint, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials, Endpoint loggedEndpoint)
            : base(apiClient, serializer, credentials, loggedEndpoint)
        {
            Ensure.Argument.IsNotNull(endpoint, nameof(endpoint));

            this.endpoint = endpoint;
        }

        public IObservable<T> TestCreateObservable<T>(Endpoint endpoint, IEnumerable<HttpHeader> headers,
            string body = "")
            => SendRequest<T>(endpoint, headers, body);

        public IObservable<string> Get()
        {
            var observable = SendRequest<string>(endpoint, AuthHeader);
            return observable;
        }
    }
}
