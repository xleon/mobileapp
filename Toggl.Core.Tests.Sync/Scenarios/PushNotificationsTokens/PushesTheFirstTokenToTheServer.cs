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
    public sealed class PushesTheFirstTokenToTheServer : ComplexSyncTest
    {
        private readonly PushNotificationsToken token;

        public PushesTheFirstTokenToTheServer()
        {
            token = new PushNotificationsToken(Guid.NewGuid().ToString());
        }

        protected override void ArrangeServices(AppServices services)
        {
            services.PushNotificationsTokenService.Token.Returns(token);
            services.KeyValueStorage.GetString(PushNotificationTokenKeys.PreviouslyRegisteredTokenKey).Returns(string.Empty);
        }

        protected override ServerState ArrangeServerState(ServerState initialServerState)
            => initialServerState;

        protected override DatabaseState ArrangeDatabaseState(ServerState serverState)
            => new DatabaseState(
                user: serverState.User.ToSyncable(),
                preferences: serverState.Preferences.ToSyncable(),
                workspaces: serverState.Workspaces.ToSyncable());

        protected override void AssertFinalState(AppServices services, ServerState finalServerState, DatabaseState finalDatabaseState)
        {
            finalServerState.PushNotificationsTokens.Should()
                .HaveCount(1)
                .And.Contain(token);
        }
    }
}
