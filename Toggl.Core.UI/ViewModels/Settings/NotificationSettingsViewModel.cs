using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Extensions;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings
{
    [Preserve(AllMembers = true)]
    public sealed class NotificationSettingsViewModel : ViewModel
    {
        private Subject<Unit> checkPermissionsSubject = new Subject<Unit>();
        public IObservable<string> ButtonTitle { get; }

        public IObservable<bool> PermissionGranted { get; }
        public IObservable<string> UpcomingEvents { get; }

        public ViewAction RequestAccess { get; }
        public ViewAction OpenUpcomingEvents { get; }

        public NotificationSettingsViewModel(
            INavigationService navigationService,
            IBackgroundService backgroundService,
            IPermissionsChecker permissionsChecker,
            IUserPreferences userPreferences,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            var permissionStatus = backgroundService.AppResumedFromBackground
                .SelectUnit()
                .Merge(checkPermissionsSubject)
                .StartWith(Unit.Default)
                .SelectMany(_ => permissionsChecker.NotificationPermissionGranted)
                .DistinctUntilChanged();

            PermissionGranted = permissionStatus
                .Select(s => s == PermissionStatus.Authorized)
                .AsDriver(schedulerProvider);

            ButtonTitle = permissionStatus
                .DistinctUntilChanged()
                .Select(buttonTitle)
                .AsDriver(schedulerProvider);

            UpcomingEvents = userPreferences.CalendarNotificationsSettings()
                .Select(s => s.Title())
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            RequestAccess = rxActionFactory.FromAsync(requestAccess);
            OpenUpcomingEvents = rxActionFactory.FromAsync(openUpcomingEvents);
        }

        private async Task requestAccess()
        {
            var status = await View.RequestNotificationAuthorization(true);
            checkPermissionsSubject.OnNext(Unit.Default);
        }

        private async Task openUpcomingEvents()
        {
            await Navigate<UpcomingEventsNotificationSettingsViewModel, Unit>();
        }

        private string buttonTitle(PermissionStatus permissionStatus)
        {
            switch (permissionStatus)
            {
                case Services.PermissionStatus.Unknown:
                    return Resources.AllowAccess;
                case Services.PermissionStatus.Rejected:
                    return Resources.OpenSettingsApp;
                default:
                    return "";
            }
        }
    }
}
