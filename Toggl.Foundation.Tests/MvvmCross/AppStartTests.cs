using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross;
using Toggl.Foundation.MvvmCross.ViewModels;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross
{
    public class AppStartTests
    {
        public abstract class AppStartTest : BaseMvvmCrossTests
        {
            protected AppStart AppStart { get; }
            protected ILoginManager LoginManager { get; } = Substitute.For<ILoginManager>();

            protected AppStartTest()
            {
                AppStart = new AppStart(LoginManager, NavigationService);
            }
        }

        public class TheConstructor : AppStartTest
        {
            [Theory]
            [InlineData(false, false)]
            [InlineData(true, false)]
            [InlineData(false, false)]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool userLoginManager, bool userNavigationService)
            {
                var loginManager = userLoginManager ? LoginManager : null;
                var navigationService = userNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new AppStart(loginManager, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheStartMethod : AppStartTest
        {
            [Fact]
            public void ShowsTheOnboardingViewModelIfTheUserHasNotLoggedInPreviously()
            {
                ITogglDataSource dataSource = null;
                LoginManager.GetDataSourceIfLoggedIn().Returns(dataSource);

                AppStart.Start();

                NavigationService.Received().Navigate<OnboardingViewModel>();
            }

            [Fact]
            public void ShowsTheTimeEntriesViewModelIfTheUserHasLoggedInPreviously()
            {
                var dataSource = Substitute.For<ITogglDataSource>();
                LoginManager.GetDataSourceIfLoggedIn().Returns(dataSource);

                AppStart.Start();

                NavigationService.Received().Navigate<TimeEntriesLogViewModel>();
            }
        }
    }
}
