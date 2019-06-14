using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage.Settings;
using static Toggl.Core.Interactors.PushNotificationTokenKeys;

namespace Toggl.Core.Interactors.PushNotifications
{
    public class SubscribeToPushNotificationsInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly TimeSpan tokenReSubmissionPeriod = TimeSpan.FromDays(5);

        private readonly IKeyValueStorage keyValueStorage;
        private readonly IPushServicesApi pushServicesApi;
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;
        private readonly ITimeService timeService;

        public SubscribeToPushNotificationsInteractor(
            IKeyValueStorage keyValueStorage,
            ITogglApi togglApi,
            IPushNotificationsTokenService pushNotificationsTokenService,
            ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.keyValueStorage = keyValueStorage;
            this.pushNotificationsTokenService = pushNotificationsTokenService;
            this.timeService = timeService;
            pushServicesApi = togglApi.PushServices;
        }

        public IObservable<Unit> Execute()
        {
            var token = pushNotificationsTokenService.Token;
            var defaultToken = default(PushNotificationsToken);
            if (!token.HasValue || (string)token.Value == (string)defaultToken)
                return Observable.Return(Unit.Default);

            var previouslyRegisteredToken = keyValueStorage.GetString(PreviouslyRegisteredTokenKey);
            var dateOfRegisteringTheToken = keyValueStorage.GetDateTimeOffset(DateOfRegisteringPreviousTokenKey);

            if (!string.IsNullOrEmpty(previouslyRegisteredToken)
                && previouslyRegisteredToken == (string)token.Value
                && dateOfRegisteringTheToken.HasValue
                && !shouldReSubmitToken(dateOfRegisteringTheToken.Value))
            {
                return Observable.Return(Unit.Default);
            }

            return pushServicesApi
                .Subscribe(token.Value)
                .Do(_ => storeRegisteredToken(token.Value.ToString()))
                .Catch((Exception ex) => Observable.Return(Unit.Default));
        }

        private bool shouldReSubmitToken(DateTimeOffset dateOfRegisteringTheToken)
            => timeService.CurrentDateTime - dateOfRegisteringTheToken >= tokenReSubmissionPeriod;

        private void storeRegisteredToken(string token)
        {
            keyValueStorage.SetString(PreviouslyRegisteredTokenKey, token);
            keyValueStorage.SetDateTimeOffset(DateOfRegisteringPreviousTokenKey, timeService.CurrentDateTime);
        }
    }
}
