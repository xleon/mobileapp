using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class SettingsViewModelTests
    {
        public class SettingsViewModelTest : BaseViewModelTests<SettingsViewModel>
        {
            protected override SettingsViewModel CreateViewModel()
                => new SettingsViewModel(DataSource, NavigationService);
        }

        public class TheConstructor : SettingsViewModelTest
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SettingsViewModel(dataSource, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheLogoutCommand : SettingsViewModelTest
        {
            [Fact]
            public async Task CallsLogoutOnTheDataSource()
            {
                await ViewModel.LogoutCommand.ExecuteAsync();

                DataSource.Received().Logout();
            }

            [Fact]
            public async Task NavigatesToTheOnboardingScreen()
            {
                await ViewModel.LogoutCommand.ExecuteAsync();

                await NavigationService.Received().Navigate<OnboardingViewModel>();
            }
        }

        public class TheBackCommand : SettingsViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.BackCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }
    }
}
