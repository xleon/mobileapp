using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Sync.Extensions;
using Toggl.Core.Tests.Sync.Helpers;
using Toggl.Core.Tests.Sync.State;
using Toggl.Shared;

namespace Toggl.Core.Tests.Sync.Scenarios.PushNotificationsTokens
{
    public sealed class RemovesInvalidToken : ComplexSyncTest
    {
        private readonly PushNotificationsToken token;

        public RemovesInvalidToken()
        {
            token = new PushNotificationsToken(Guid.NewGuid().ToString());
        }

        protected override void ArrangeServices(AppServices services)
        {
            services.PushNotificationsTokenService.Token.Returns(token);
            services.KeyValueStorage.GetString(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey).Returns(token.ToString());
        }

        protected override ServerState ArrangeServerState(ServerState initialServerState)
            => initialServerState.With(pushNotificationTokens: new[] { token });

        protected override DatabaseState ArrangeDatabaseState(ServerState serverState)
            => new DatabaseState(
                user: serverState.User.ToSyncable(),
                preferences: serverState.Preferences.ToSyncable(),
                workspaces: serverState.Workspaces.ToSyncable());

        protected override async Task Act(ISyncManager syncManager, AppServices services)
        {
            await new UnsubscribeFromPushNotificationsInteractor(
                    services.PushNotificationsTokenService,
                    services.KeyValueStorage,
                    services.TogglApi)
                .Execute();
        }

        protected override void AssertFinalState(AppServices services, ServerState finalServerState, DatabaseState finalDatabaseState)
        {
            finalServerState.PushNotificationsTokens.Should().HaveCount(0);
        }
    }
}
