using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Clients
{
    internal sealed class ClientsClient : BaseClient, IClientsClient
    {
        private readonly ClientEndpoints endPoints;

        public ClientsClient(ClientEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials)
        {
            this.endPoints = endPoints;
        }

        public IObservable<List<Client>> GetAll()
        {
            var observable = CreateObservable<List<Client>>(endPoints.Get, AuthHeader);
            return observable;
        }

        public IObservable<Client> Create(Client client)
        {
            var endPoint = endPoints.Post(client.WorkspaceId);
            var observable = CreateObservable<Client>(endPoint, AuthHeader, client, SerializationReason.Post);
            return observable;
        }
    }
}
