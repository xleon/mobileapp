using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;
using Toggl.Shared.Extensions;

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
            => SendRequest(endPoints.Subscribe, AuthHeader, json(token)).SelectUnit();

        public IObservable<Unit> Unsubscribe(PushNotificationsToken token)
            => Observable.Return(Unit.Default);

        private string json(PushNotificationsToken token)
            => $"{{\"fcm_registration_token\": \"{(string)token}\"}}";
    }
}
