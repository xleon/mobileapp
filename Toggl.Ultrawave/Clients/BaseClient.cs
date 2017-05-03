using System;
using System.Collections.Generic;
using System.Text;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;
using Toggl.Multivac;

namespace Toggl.Ultrawave.Clients
{
    internal abstract class BaseClient
    {
        private readonly IApiClient apiClient;
        private readonly IJsonSerializer serializer;

        protected BaseClient(IApiClient apiClient, IJsonSerializer serializer)
        {
            Ensure.ArgumentIsNotNull(apiClient, nameof(apiClient));
            Ensure.ArgumentIsNotNull(serializer, nameof(serializer));

            this.apiClient = apiClient;
            this.serializer = serializer;
        }

        protected HttpHeader GetAuthHeader(string username, string password)
        {
            var authString = $"{username}:{password}";
            var authStringBytes = Encoding.UTF8.GetBytes(authString);
            var authHeader = Convert.ToBase64String(authStringBytes);
                                    
            return new HttpHeader("Authorization", authHeader, HttpHeader.HeaderType.Auth);
        }

        protected ICall<T> CreateCall<T>(Endpoint endpoint, HttpHeader header, string body = "")
            => CreateCall<T>(endpoint, new [] { header }, body);

        protected ICall<T> CreateCall<T>(Endpoint endpoint, IEnumerable<HttpHeader> headers, string body = "")
        {
            var request = new Request(body, endpoint.Url, headers, endpoint.Method);
            var call = new Call<T>(request, apiClient, serializer);
            return call;
        }
    }
}
