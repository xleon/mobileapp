using System;
using System.Net.Http;
using System.Collections.Generic;
using Toggl.Ultrawave.Network;
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
    }
}
