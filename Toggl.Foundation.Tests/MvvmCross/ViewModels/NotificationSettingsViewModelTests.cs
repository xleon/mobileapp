using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.Settings;
using Toggl.Foundation.Tests.Generators;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class NotificationSettingsViewModelTests
    {
        public abstract class NotificationSettingsViewModelTest : BaseViewModelTests<NotificationSettingsViewModel>
        {
            protected override NotificationSettingsViewModel CreateViewModel()
                => new NotificationSettingsViewModel(NavigationService, PermissionsService, UserPreferences);
        }

        public sealed class TheConstructor : NotificationSettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool usePermissionsService,
                bool useUserPreferences)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new NotificationSettingsViewModel(
                        useNavigationService ? NavigationService : null,
                        usePermissionsService ? PermissionsService : null,
                        useUserPreferences ? UserPreferences : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class ThePermissionGrantedProperty : NotificationSettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task GetsInitialisedToTheProperValue(bool permissionGranted)
            {
                PermissionsService.NotificationPermissionGranted.Returns(Observable.Return(permissionGranted));

                await ViewModel.Initialize();

                ViewModel.PermissionGranted.Should().Be(permissionGranted);
            }
        }

        public sealed class TheRequestAccessAction : NotificationSettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task OpensAppSettings()
            {
                await ViewModel.RequestAccessAction.Execute(Unit.Default);

                PermissionsService.Received().OpenAppSettings();
            }
        }

        public sealed class TheOpenUpcomingEventsAction : NotificationSettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(UpcomingEventsOption.Disabled, false, 0)]
            [InlineData(UpcomingEventsOption.WhenEventStarts, true, 0)]
            [InlineData(UpcomingEventsOption.FiveMinutes, true, 5)]
            [InlineData(UpcomingEventsOption.TenMinutes, true, 10)]
            [InlineData(UpcomingEventsOption.FifteenMinutes, true, 15)]
            [InlineData(UpcomingEventsOption.ThirtyMinutes, true, 30)]
            [InlineData(UpcomingEventsOption.OneHour, true, 60)]
            public async Task UpdatesTheCalendarNotificationsSettingsWhenAnOptionIsSelected(UpcomingEventsOption option, bool enabled, int minutes)
            {
                NavigationService
                    .Navigate<UpcomingEventsNotificationSettingsViewModel,
                        SelectFromListParameters<UpcomingEventsOption>,
                        UpcomingEventsOption>(Arg.Any<SelectFromListParameters<UpcomingEventsOption>>())
                    .Returns(option);

                await ViewModel.OpenUpcomingEvents.Execute(Unit.Default);

                UserPreferences.Received().SetCalendarNotificationsEnabled(enabled);

                if (enabled)
                    UserPreferences.Received().SetTimeSpanBeforeCalendarNotifications(Arg.Is<TimeSpan>(arg => arg == TimeSpan.FromMinutes(minutes)));
                else
                    UserPreferences.DidNotReceive().SetTimeSpanBeforeCalendarNotifications(Arg.Any<TimeSpan>());
            }
        }
    }
}
