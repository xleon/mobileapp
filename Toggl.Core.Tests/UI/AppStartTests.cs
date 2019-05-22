using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Storage.Settings;
using Toggl.Networking;
using Xunit;
using System.Reactive;
using FluentAssertions;

namespace Toggl.Core.Tests.UI
{
    public sealed class AppStartTests
    {
        public abstract class AppStartTest : BaseTest
        {
            protected App<OnboardingViewModel, Unit> App { get; }
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

                App = new App<OnboardingViewModel, Unit>(dependencyContainer);
            }
        }

        public class TheConstructor : AppStartTest
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

                var _ = new App<OnboardingViewModel, Unit>(dependencyContainer);

                OnboardingStorage.Received().SetFirstOpened(TimeService.CurrentDateTime);
            }

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

                var _ = new App<OnboardingViewModel, Unit>(dependencyContainer);

                OnboardingStorage.Received().SetLastOpened(now);
                OnboardingStorage.Received().SetIsNewUser(false);
            }
        }

        public sealed class TheNavigateIfUserDoesNotHaveFullAccessMethod : AppStartTest
        {
            [Fact, LogIfTooSlow]
            public async Task ShowsTheOutdatedViewIfTheCurrentVersionOfTheAppIsOutdated()
            {
                AccessRestrictionStorage.IsClientOutdated().Returns(true);

                App.NavigateIfUserDoesNotHaveFullAccess();

                await NavigationService.Received().Navigate<OutdatedAppViewModel>(null);
                UserAccessManager.DidNotReceive().CheckIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheOutdatedViewIfTheVersionOfTheCurrentlyUsedApiIsOutdated()
            {
                AccessRestrictionStorage.IsApiOutdated().Returns(true);

                App.NavigateIfUserDoesNotHaveFullAccess();

                await NavigationService.Received().Navigate<OutdatedAppViewModel>(null);
                UserAccessManager.DidNotReceive().CheckIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheReLoginViewIfTheUserRevokedTheApiToken()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);

                App.NavigateIfUserDoesNotHaveFullAccess();

                await NavigationService.Received().Navigate<TokenResetViewModel>(null);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheOutdatedViewIfTheTokenWasRevokedAndTheAppIsOutdated()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);
                AccessRestrictionStorage.IsClientOutdated().Returns(true);

                App.NavigateIfUserDoesNotHaveFullAccess();

                await NavigationService.Received().Navigate<OutdatedAppViewModel>(null);
                UserAccessManager.DidNotReceive().CheckIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheOutdatedViewIfTheTokenWasRevokedAndTheApiIsOutdated()
            {
                AccessRestrictionStorage.IsUnauthorized(Arg.Any<string>()).Returns(true);
                AccessRestrictionStorage.IsApiOutdated().Returns(true);

                App.NavigateIfUserDoesNotHaveFullAccess();

                await NavigationService.Received().Navigate<OutdatedAppViewModel>(null);
                await NavigationService.DidNotReceive().Navigate<TokenResetViewModel>(null);
                UserAccessManager.DidNotReceive().CheckIfLoggedIn();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotShowTheUnauthorizedAccessViewIfUsersApiTokenChanged()
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

                var result = App.NavigateIfUserDoesNotHaveFullAccess();

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheOnboardingViewModelIfTheUserHasNotLoggedInPreviously()
            {
                UserAccessManager.CheckIfLoggedIn().Returns(false);

                App.NavigateIfUserDoesNotHaveFullAccess();

                await NavigationService.Received().Navigate<OnboardingViewModel>(null);
            }

            [Fact, LogIfTooSlow]
            public void CallsNavigateToMainTabBarViewModelIfTheUserHasLoggedInPreviously()
            {
                var result = App.NavigateIfUserDoesNotHaveFullAccess();

                result.Should().BeTrue();
            }
        }
    }
}
