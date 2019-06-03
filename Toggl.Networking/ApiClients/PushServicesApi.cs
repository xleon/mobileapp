using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;

namespace Toggl.Networking.ApiClients
{
    internal sealed class PushServicesApi : BaseApi, IPushServicesApi
    {
        private readonly PushServicesEndpoints endPoints;

        internal PushServicesApi(
            Endpoints endPoints,
            IApiClient apiClient,
            IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.PushServices;
        }

        public IObservable<Unit> Subscribe(PushNotificationsToken token)
            => Observable.Return(Unit.Default);

        public IObservable<Unit> Unsubscribe(PushNotificationsToken token)
            => Observable.Return(Unit.Default);

        private string json(PushNotificationsToken token)
            => $"{{\"fcm_token\": \"{(string)token}\"}}";
    }
}
