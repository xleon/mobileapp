using System;
using Toggl.Ultrawave.Models;
using Toggl.Ultrawave.Network;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.ApiClients
{
    internal sealed class UserApi : BaseApi, IUserApi
    {
        private readonly UserEndpoints endPoints;

        public UserApi(UserEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials)
        {
            this.endPoints = endPoints;
        }

        public IObservable<User> Get()
            => CreateObservable<User>(endPoints.Get, AuthHeader);
    }
}
