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
using static Toggl.Core.Interactors.Settings.GetFeedbackInfoInteractor;

namespace Toggl.Core.Tests.Interactors.Settings
{
    public sealed class GetFeedbackInfoInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheParametersIsNullOrInvalid(
                bool useUserDataSource,
                bool useWorkspacesDataSource,
                bool useTimeEntriesDataSource,
                bool useplatformInfo,
                bool useUserPreferences,
                bool useLastTimeUsageStorage,
                bool useTimeService,
                bool useInteractorFactory)
            {
                // ReSharper disable once ObjectCreationAsStatement
                Action createInstance = () => new GetFeedbackInfoInteractor(
                    useUserDataSource ? DataSource.User : null,
                    useWorkspacesDataSource ? DataSource.Workspaces : null,
                    useTimeEntriesDataSource ? DataSource.TimeEntries : null,
                    useplatformInfo ? Substitute.For<IPlatformInfo>() : null,
                    useUserPreferences ? UserPreferences : null,
                    useLastTimeUsageStorage ? Substitute.For<ILastTimeUsageStorage>() : null,
                    useTimeService ? TimeService : null,
                    useInteractorFactory ? InteractorFactory : null);

                createInstance.Should().Throw<ArgumentException>();
            }
        }

        public sealed class TheGetMethod : BaseInteractorTests
        {
            public static IEnumerable<object[]> LoginAndSyncTimesData
                => new[]
                {
                    new object[] { DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1), DateTimeOffset.Now.AddHours(1) },
                    new object[] { null, DateTimeOffset.Now.AddHours(1), DateTimeOffset.Now.AddHours(1) },
                    new object[] { DateTimeOffset.Now, null, DateTimeOffset.Now.AddHours(1) },
                    new object[] { DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1), null },
                    new object[] { DateTimeOffset.Now, null, null },
                    new object[] { null, DateTimeOffset.Now.AddHours(1), null },
                    new object[] { null, null, DateTimeOffset.Now.AddHours(1) },
                    new object[] { null, null, null },
                };

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

            private readonly IThreadSafeUser user = Substitute.For<IThreadSafeUser>();

            public TheGetMethod()
            {
                DataSource.User.Get().Returns(Observable.Return(user));
                DataSource.Workspaces.GetAll().Returns(Observable.Return(workspaces));
                DataSource.TimeEntries.GetAll().Returns(Observable.Return(timeEntries));
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo => Observable.Return(timeEntries.Where<IThreadSafeTimeEntry>(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));
            }

            [Fact, LogIfTooSlow]
            public async Task GetsCorrectplatformInfo()
            {
                var operatingSystem = "TogglOS";
                var phoneModel = "TogglPhone";
                PlatformInfo.OperatingSystem.Returns(operatingSystem);
                PlatformInfo.PhoneModel.Returns(phoneModel);

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[GetFeedbackInfoInteractor.OperatingSystem].Should().BeSameAs(operatingSystem);
                result[PhoneModel].Should().Be(phoneModel);
            }

            [Fact, LogIfTooSlow]
            public async Task GetsTheAppPlatformSlashAppVersion()
            {
                const string version = "42.2";
                var platform = Platform.Giskard;
                PlatformInfo.Version.Returns(version);
                PlatformInfo.Platform.Returns(platform);
                var formattedUserAgent = $"{platform}/{version}";

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[AppNameAndVersion].Should().Be(formattedUserAgent);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(LoginAndSyncTimesData))]
            public async Task GetsTimesOfLastUsage(
                DateTimeOffset? login,
                DateTimeOffset? syncAttempt,
                DateTimeOffset? successfulSync)
            {
                LastTimeUsageStorage.LastLogin.Returns(login);
                LastTimeUsageStorage.LastSyncAttempt.Returns(syncAttempt);
                LastTimeUsageStorage.LastSuccessfulSync.Returns(successfulSync);

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[LastLogin].Should().Be(login.HasValue ? login.ToString() : "never");
                result[LastSyncAttempt].Should().Be(syncAttempt.HasValue ? syncAttempt.ToString() : "never");
                result[LastSuccessfulSync].Should().Be(successfulSync.HasValue ? successfulSync.ToString() : "never");
            }

            [Fact, LogIfTooSlow]
            public async Task GetsCurrentDeviceTime()
            {
                var now = DateTime.Now;
                TimeService.CurrentDateTime.Returns(now);

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[DeviceTime].Should().Be(now.ToString("MM/dd/yyyy HH:mm:ss K"));
            }

            [Theory, LogIfTooSlow]
            [InlineData(false)]
            [InlineData(true)]
            public async Task GetsUsersPreferences(bool isManualModeEnabled)
            {
                UserPreferences.IsManualModeEnabled.Returns(isManualModeEnabled);

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[ManualModeIsOn].Should().Be(isManualModeEnabled ? "yes" : "no");
            }

            [Theory, LogIfTooSlow]
            [InlineData(1)]
            public async Task GetsTheUserId(int userId)
            {
                var user = Substitute.For<IThreadSafeUser>();
                user.Id.Returns(userId);
                InteractorFactory.GetCurrentUser().Execute().Returns(Observable.Return(user));

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[UserId].Should().Be(userId.ToString());
            }

            [Fact, LogIfTooSlow]
            public async Task GetsApplicationInstallLocation()
            {
                var installLocation = new ApplicationInstallLocation();
                PlatformInfo.InstallLocation.Returns(installLocation);
                PlatformInfo.Platform.Returns(Platform.Giskard);

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[InstallLocation].Should().Be(installLocation.ToString());
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotGetApplicationInstallLocationOnIOs()
            {
                var installLocation = new ApplicationInstallLocation();
                PlatformInfo.InstallLocation.Returns(installLocation);
                PlatformInfo.Platform.Returns(Platform.Daneel);

                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result.Should().NotContainKey(InstallLocation);
            }

            [Fact, LogIfTooSlow]
            public async Task CountsAllWorkspaces()
            {
                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[NumberOfWorkspaces].Should().Be("4");
            }

            [Fact, LogIfTooSlow]
            public async Task CountsTimeEntries()
            {
                var result = await InteractorFactory.GetFeedbackInfo().Execute();

                result[NumberOfTimeEntries].Should().Be("6");
                result[NumberOfUnsyncedTimeEntries].Should().Be("1");
                result[NumberOfUnsyncableTimeEntries].Should().Be("2");
            }
        }
    }
}
