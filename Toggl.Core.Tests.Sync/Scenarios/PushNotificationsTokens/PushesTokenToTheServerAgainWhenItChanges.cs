using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Tests.Sync.Extensions;
using Toggl.Core.Tests.Sync.Helpers;
using Toggl.Core.Tests.Sync.State;
using Toggl.Shared;

namespace Toggl.Core.Tests.Sync.Scenarios.PushNotificationsTokens
{
    public sealed class PushesTokenToTheServerAgainWhenItChanges : ComplexSyncTest
    {
        private readonly PushNotificationsToken oldToken;
        private readonly PushNotificationsToken token;

        public PushesTokenToTheServerAgainWhenItChanges()
        {
            oldToken = new PushNotificationsToken(Guid.NewGuid().ToString());
            token = new PushNotificationsToken(Guid.NewGuid().ToString());
        }

        protected override void ArrangeServices(AppServices services)
        {
            services.PushNotificationsTokenService.Token.Returns(token);
            services.KeyValueStorage.GetString(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey)
                .Returns(oldToken.ToString());
        }

        protected override ServerState ArrangeServerState(ServerState initialServerState)
            => initialServerState.With(pushNotificationTokens: new[] { oldToken });

        protected override DatabaseState ArrangeDatabaseState(ServerState serverState)
            => new DatabaseState(
                user: serverState.User.ToSyncable(),
                preferences: serverState.Preferences.ToSyncable(),
                workspaces: serverState.Workspaces.ToSyncable());

        protected override void AssertFinalState(AppServices services, ServerState finalServerState, DatabaseState finalDatabaseState)
        {
            finalServerState.PushNotificationsTokens.Should()
                .HaveCount(2)
                .And.Contain(oldToken)
                .And.Contain(token);
        }
    }
}
