using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings
{
    [Preserve(AllMembers = true)]
    public sealed class NotificationSettingsViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService navigationService;
        private readonly IPermissionsService permissionsService;
        private readonly IUserPreferences userPreferences;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IRxActionFactory rxActionFactory;

        public IObservable<bool> PermissionGranted;
        public IObservable<string> UpcomingEvents;

        public UIAction RequestAccess { get; }
        public UIAction OpenUpcomingEvents { get; }

        public NotificationSettingsViewModel(
            IMvxNavigationService navigationService,
            IBackgroundService backgroundService,
            IPermissionsService permissionsService,
            IUserPreferences userPreferences,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;
            this.permissionsService = permissionsService;
            this.userPreferences = userPreferences;
            this.schedulerProvider = schedulerProvider;
            this.rxActionFactory = rxActionFactory;

            PermissionGranted = backgroundService.AppResumedFromBackground
                .SelectUnit()
                .StartWith(Unit.Default)
                .SelectMany(_ => permissionsService.NotificationPermissionGranted)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            UpcomingEvents = userPreferences.CalendarNotificationsSettings()
                .Select(s => s.Title())
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            RequestAccess = rxActionFactory.FromAction(requestAccess);
            OpenUpcomingEvents = rxActionFactory.FromAsync(openUpcomingEvents);
        }

        private void requestAccess()
        {
            permissionsService.OpenAppSettings();
        }

        private async Task openUpcomingEvents()
        {
            await navigationService.Navigate<UpcomingEventsNotificationSettingsViewModel, Unit>();
        }
    }
}
