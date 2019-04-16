using System;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Networking.ApiClients.Interfaces;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;

namespace Toggl.Networking.ApiClients
{
    internal sealed class LocationApi : BaseApi, ILocationApi
    {
        private readonly LocationEndpoints endPoints;

        public LocationApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            Ensure.Argument.IsNotNull(endPoints, nameof(endPoints));

            this.endPoints = endPoints.Location;
        }

        public IObservable<ILocation> Get()
            => SendRequest<Location>(endPoints.Get, AuthHeader);
    }
}
