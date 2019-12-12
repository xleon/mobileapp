using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors.Settings;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;
using Toggl.Storage.Settings;
using Xunit;
using static Toggl.Core.Interactors.Settings.SendFeedbackInteractor;

namespace Toggl.Core.Tests.Interactors.Settings
{
    public sealed class SendFeedbackInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheParametersIsNullOrInvalid(
                bool useFeedbackApi,
                bool useUserDataSource,
                bool useInteractorFactory,
                bool useMessage)
            {
                // ReSharper disable once ObjectCreationAsStatement
                Action createInstance = () => new SendFeedbackInteractor(
                    useFeedbackApi ? Substitute.For<IFeedbackApi>() : null,
                    useUserDataSource ? DataSource.User : null,
                    useInteractorFactory ? InteractorFactory : null,
                    useMessage ? "some message" : null);

                createInstance.Should().Throw<ArgumentException>();
            }
        }

        public sealed class TheSendMethod : BaseInteractorTests
        {
            private readonly IEnumerable<IThreadSafeTimeEntry> timeEntries = new[]
            {
                new MockTimeEntry { SyncStatus = SyncStatus.InSync },
                new MockTimeEntry { SyncStatus = SyncStatus.SyncNeeded },
                new MockTimeEntry { SyncStatus = SyncStatus.InSync },
                new MockTimeEntry { SyncStatus = SyncStatus.SyncFailed },
                new MockTimeEntry { SyncStatus = SyncStatus.SyncFailed },
                new MockTimeEntry { SyncStatus = SyncStatus.InSync },
            };

            private readonly IEnumerable<IThreadSafeWorkspace> workspaces = new[]
            {
                new MockWorkspace(), new MockWorkspace(), new MockWorkspace(), new MockWorkspace()
            };

            private readonly IFeedbackApi feedbackApi = Substitute.For<IFeedbackApi>();

            private readonly IThreadSafeUser user = Substitute.For<IThreadSafeUser>();

            public TheSendMethod()
            {
                DataSource.User.Get().Returns(Observable.Return(user));
                DataSource.Workspaces.GetAll().Returns(Observable.Return(workspaces));
                DataSource.TimeEntries.GetAll().Returns(Observable.Return(timeEntries));
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo => Observable.Return(timeEntries.Where<IThreadSafeTimeEntry>(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));
            }

            [Property]
            public void SendsUsersMessage(NonNull<string> message)
            {
                var email = $"{Guid.NewGuid().ToString()}@randomdomain.com".ToEmail();
                user.Email.Returns(email);

                executeInteractor(message: message.Get).Wait();

                feedbackApi.Received().Send(Arg.Is(email), Arg.Is(message.Get), Arg.Any<Dictionary<string, string>>());
            }

            private async Task executeInteractor(string message = "")
            {
                var interactor = new SendFeedbackInteractor(
                    feedbackApi,
                    DataSource.User,
                    InteractorFactory,
                    message);

                await interactor.Execute();
            }
        }
    }
}
