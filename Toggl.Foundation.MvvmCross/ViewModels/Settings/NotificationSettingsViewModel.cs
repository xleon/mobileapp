using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Settings;
using Toggl.Foundation.MvvmCross.Helper;
using FoundationResources = Toggl.Foundation.Resources;

namespace Toggl.Foundation.MvvmCross.ViewModels.Settings
{
    [Preserve(AllMembers = true)]
    public sealed class NotificationSettingsViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService navigationService;
        private readonly IPermissionsService permissionsService;
        private readonly IUserPreferences userPreferences;

        public bool PermissionGranted { get; private set; }

        public IObservable<string> UpcomingEvents { get; }

        public UIAction RequestAccessAction { get; }

        public UIAction OpenUpcomingEvents { get; }

        public NotificationSettingsViewModel(
            IMvxNavigationService navigationService,
            IPermissionsService permissionsService,
            IUserPreferences userPreferences)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));

            this.navigationService = navigationService;
            this.permissionsService = permissionsService;
            this.userPreferences = userPreferences;

            RequestAccessAction = new UIAction(requestAccess);
            OpenUpcomingEvents = new UIAction(openUpcomingEvents);
        }

        public override async Task Initialize()
        {
            PermissionGranted = await permissionsService.NotificationPermissionGranted;
            await base.Initialize();
        }

        private IObservable<Unit> requestAccess()
        {
            permissionsService.OpenAppSettings();
            return Observable.Return(Unit.Default);
        }

        private IObservable<Unit> openUpcomingEvents()
        {
            var options = new List<SelectableItem<UpcomingEventsOption>>
            {
                new SelectableItem<UpcomingEventsOption> { Title = FoundationResources.Disabled, Value = UpcomingEventsOption.Disabled },
                new SelectableItem<UpcomingEventsOption> { Title = FoundationResources.WhenEventStarts, Value = UpcomingEventsOption.WhenEventStarts },
                new SelectableItem<UpcomingEventsOption> { Title = FoundationResources.FiveMinutes, Value = UpcomingEventsOption.FiveMinutes },
                new SelectableItem<UpcomingEventsOption> { Title = FoundationResources.TenMinutes, Value = UpcomingEventsOption.TenMinutes },
                new SelectableItem<UpcomingEventsOption> { Title = FoundationResources.FifteenMinutes, Value = UpcomingEventsOption.FifteenMinutes },
                new SelectableItem<UpcomingEventsOption> { Title = FoundationResources.ThirtyMinutes, Value = UpcomingEventsOption.ThirtyMinutes },
                new SelectableItem<UpcomingEventsOption> { Title = FoundationResources.OneHour, Value = UpcomingEventsOption.OneHour },
            };

            return Observable.FromAsync(async () =>
            {
                var selectedIndex = await currentUpcomingEventsOptionIndex();
                var parameters = new SelectFromListParameters<UpcomingEventsOption>
                {
                    Items = options,
                    SelectedIndex = selectedIndex,
                };

                var selection = await navigationService
                    .Navigate<UpcomingEventsNotificationSettingsViewModel, SelectFromListParameters<UpcomingEventsOption>, UpcomingEventsOption>(parameters);
                saveUpcomingEventsOption(selection);
            }).SelectUnit();
        }

        private IObservable<int> currentUpcomingEventsOptionIndex()
        {
            return Observable.FromAsync(async () =>
            {
                var notificationsEnabled = await userPreferences.CalendarNotificationsEnabled;
                var timeSpan = await userPreferences.TimeSpanBeforeCalendarNotifications;

                if (!notificationsEnabled)
                    return 0;
                if (timeSpan == TimeSpan.Zero)
                    return 1;
                if (timeSpan == TimeSpan.FromMinutes(5))
                    return 2;
                if (timeSpan == TimeSpan.FromMinutes(10))
                    return 3;
                if (timeSpan == TimeSpan.FromMinutes(15))
                    return 4;
                if (timeSpan == TimeSpan.FromMinutes(30))
                    return 5;
                return 6;
            });
        }

        private void saveUpcomingEventsOption(UpcomingEventsOption option)
        {
            userPreferences.SetCalendarNotificationsEnabled(option != UpcomingEventsOption.Disabled);

            switch (option)
            {
                case UpcomingEventsOption.WhenEventStarts:
                    userPreferences.SetTimeSpanBeforeCalendarNotifications(TimeSpan.Zero);
                    break;
                case UpcomingEventsOption.FiveMinutes:
                    userPreferences.SetTimeSpanBeforeCalendarNotifications(TimeSpan.FromMinutes(5));
                    break;
                case UpcomingEventsOption.TenMinutes:
                    userPreferences.SetTimeSpanBeforeCalendarNotifications(TimeSpan.FromMinutes(10));
                    break;
                case UpcomingEventsOption.FifteenMinutes:
                    userPreferences.SetTimeSpanBeforeCalendarNotifications(TimeSpan.FromMinutes(15));
                    break;
                case UpcomingEventsOption.ThirtyMinutes:
                    userPreferences.SetTimeSpanBeforeCalendarNotifications(TimeSpan.FromMinutes(30));
                    break;
                case UpcomingEventsOption.OneHour:
                    userPreferences.SetTimeSpanBeforeCalendarNotifications(TimeSpan.FromHours(1));
                    break;
            }
        }
    }
}
