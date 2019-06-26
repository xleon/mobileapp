using FluentAssertions;
using NSubstitute;
using System;
using System.Reactive.Linq;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Core.UI;
using Toggl.Networking;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.UI
{
    public sealed class AppStartTests
    {
        public abstract class AppStartTest : BaseTest
        {
            protected AppStart App { get; }
            protected IUserAccessManager UserAccessManager { get; } = Substitute.For<IUserAccessManager>();
            protected IOnboardingStorage OnboardingStorage { get; } = Substitute.For<IOnboardingStorage>();
            protected IAccessRestrictionStorage AccessRestrictionStorage { get; } =
                Substitute.For<IAccessRestrictionStorage>();

            protected AppStartTest()
            {
                var api = Substitute.For<ITogglApi>();
                UserAccessManager.UserLoggedIn.Returns(Observable.Return(api));

                var dependencyContainer = new TestDependencyContainer
                {
                    MockTimeService = TimeService,
                    MockUserAccessManager = UserAccessManager,
                    MockNavigationService = NavigationService,
                    MockOnboardingStorage = OnboardingStorage,
                    MockAccessRestrictionStorage = AccessRestrictionStorage,
                    MockSyncManager = Substitute.For<ISyncManager>(),
                    MockInteractorFactory = Substitute.For<IInteractorFactory>(),
                    MockBackgroundSyncService = Substitute.For<IBackgroundSyncService>()
                };
                UserAccessManager.CheckIfLoggedIn().Returns(true);

                App = new AppStart(dependencyContainer);
            }
        }

        public class TheConstructor : AppStartTest
        {
            [Fact, LogIfTooSlow]
            public void MarksTheUserAsNotNewWhenUsingTheAppForTheFirstTimeAfterSixtyDays()
            {
                var now = DateTimeOffset.Now;

                TimeService.CurrentDateTime.Returns(now);
                OnboardingStorage.GetLastOpened().Returns(now.AddDays(-60));
                var dependencyContainer = new TestDependencyContainer
                {
                    MockTimeService = TimeService,
                    MockUserAccessManager = UserAccessManager,
                    MockNavigationService = NavigationService,
                    MockOnboardingStorage = OnboardingStorage,
                    MockAccessRestrictionStorage = AccessRestrictionStorage,
                    MockSyncManager = Substitute.For<ISyncManager>(),
                    MockInteractorFactory = Substitute.For<IInteractorFactory>(),
                    MockBackgroundSyncService = Substitute.For<IBackgroundSyncService>()
                };

                var app = new AppStart(dependencyContainer);
                app.UpdateOnboardingProgress();

                OnboardingStorage.Received().SetLastOpened(now);
                OnboardingStorage.Received().SetIsNewUser(false);
            }
        }

        public class TheSetFirstOpenedMethod : AppStartTest
        {
            [Fact, LogIfTooSlow]
            public void SetsFirstOpenedTime()
            {
                TimeService.CurrentDateTime.Returns(new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero));
                var dependencyContainer = new TestDependencyContainer
                {
                    MockTimeService = TimeService,
                    MockUserAccessManager = UserAccessManager,
                    MockNavigationService = NavigationService,
                    MockOnboardingStorage = OnboardingStorage,
                    MockAccessRestrictionStorage = AccessRestrictionStorage,
                    MockSyncManager = Substitute.For<ISyncManager>(),
                    MockInteractorFactory = Substitute.For<IInteractorFactory>(),
                    MockBackgroundSyncService = Substitute.For<IBackgroundSyncService>()
                };
                var appStart = new AppStart(dependencyContainer);

                appStart.SetFirstOpened();

                OnboardingStorage.Received().SetFirstOpened(TimeService.CurrentDateTime);
            }
        }

        public sealed class TheGetAccessLevelMethod : AppStartTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsAccessRestrictedWhenTheCurrentVersionOfTheAppIsOutdated()
            {
                AccessRestrictionStorage.IsClientOutdated().Returns(true);

                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.AccessRestricted);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAccessRestrictedWhenTheVersionOfTheCurrentlyUsedApiIsOutdated()
            {
                AccessRestrictionStorage.IsApiOutdated().Returns(true);

                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.AccessRestricted);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTokenRevokedWhenTheUserRevokedTheApiToken()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);

                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.TokenRevoked);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAccessRestrictedWhenTheTokenWasRevokedAndTheAppIsOutdated()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);
                AccessRestrictionStorage.IsClientOutdated().Returns(true);

                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.AccessRestricted);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAccessRestrictedWhenTheTokenWasRevokedAndTheApiIsOutdated()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);
                AccessRestrictionStorage.IsApiOutdated().Returns(true);

                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.AccessRestricted);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsLoggedInWhenUsersApiTokenChanged()
            {
                var oldApiToken = Guid.NewGuid().ToString();
                var newApiToken = Guid.NewGuid().ToString();
                var user = Substitute.For<IThreadSafeUser>();
                var interactorFactory = Substitute.For<IInteractorFactory>();
                user.ApiToken.Returns(newApiToken);
                interactorFactory.GetCurrentUser().Execute().Returns(Observable.Return(user));
                AccessRestrictionStorage.IsUnauthorized(Arg.Is(oldApiToken)).Returns(true);
                AccessRestrictionStorage.IsApiOutdated().Returns(false);
                AccessRestrictionStorage.IsClientOutdated().Returns(false);

                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.LoggedIn);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsNotLoggedInWhenTheUserHasNotLoggedInPreviously()
            {
                UserAccessManager.CheckIfLoggedIn().Returns(false);

                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.NotLoggedIn);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsLoggedInWhenTheUserHasLoggedInPreviously()
            {
                var accessLevel = App.GetAccessLevel();
                accessLevel.Should().Be(AccessLevel.LoggedIn);
            }
        }
    }
}
