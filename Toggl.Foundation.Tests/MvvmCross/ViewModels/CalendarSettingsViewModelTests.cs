using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels.Settings;
using Toggl.Foundation.Tests.Generators;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class CalendarSettingsViewModelTests
    {
        public abstract class CalendarSettingsViewModelTest : BaseViewModelTests<CalendarSettingsViewModel>
        {
            protected override CalendarSettingsViewModel CreateViewModel()
                => new CalendarSettingsViewModel(InteractorFactory, PermissionsService);
        }

        public sealed class TheConstructor : CalendarSettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useInteractorFactory,
                bool usePermissionsService)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarSettingsViewModel(
                        useInteractorFactory ? InteractorFactory : null,
                        usePermissionsService ? PermissionsService : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class ThePermissionGrantedProperty : CalendarSettingsViewModelTest
        {
            [Property]
            public void GetsInitialisedToTheProperValue(bool permissionGranted)
            {
                PermissionsService.CalendarPermissionGranted.Returns(permissionGranted);
                var viewModel = CreateViewModel();

                viewModel.PermissionGranted.Should().Be(permissionGranted);
            }
        }

        public sealed class TheRequestAccessAction : CalendarSettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task OpensAppSettings()
            {
                await ViewModel.RequestAccessAction.Execute(Unit.Default);

                PermissionsService.Received().OpenAppSettings();
            }
        }
    }
}
