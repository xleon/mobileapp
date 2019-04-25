using System;
using System.Collections.Generic;
using Toggl.Shared.Models;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;

namespace Toggl.Networking.ApiClients
{
    internal sealed class ClientsApi : BaseApi, IClientsApi
    {
        private readonly ClientEndpoints endPoints;

        public ClientsApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Clients;
        }

        public IObservable<List<IClient>> GetAll()
            => SendRequest<Client, IClient>(endPoints.Get, AuthHeader);

        public IObservable<List<IClient>> GetAllSince(DateTimeOffset threshold)
            => SendRequest<Client, IClient>(endPoints.GetSince(threshold), AuthHeader);

        public IObservable<IClient> Create(IClient client)
        {
            var endPoint = endPoints.Post(client.WorkspaceId);
            var clientCopy = client as Client ?? new Client(client);
            var observable = SendRequest(endPoint, AuthHeader, clientCopy, SerializationReason.Post);
            return observable;
        }
    }
}
