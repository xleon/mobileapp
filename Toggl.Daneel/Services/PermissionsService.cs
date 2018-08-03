using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EventKit;
using Foundation;
using Toggl.Foundation.MvvmCross.Services;
using UIKit;

namespace Toggl.Daneel.Services
{
    [Preserve(AllMembers = true)]
    public sealed class PermissionsService : IPermissionsService
    {
        private readonly BehaviorSubject<bool> calendarAuthorizationStatusSubject;

        public IObservable<bool> CalendarAuthorizationStatus { get; }

        public PermissionsService()
        {
            calendarAuthorizationStatusSubject = new BehaviorSubject<bool>(isCalendarAuthorized());
            CalendarAuthorizationStatus = calendarAuthorizationStatusSubject
                .AsObservable()
                .DistinctUntilChanged();
        }

        #region Calendar

        public void RequestCalendarAuthorization(bool force = false)
        {
            switch (EKEventStore.GetAuthorizationStatus(EKEntityType.Event))
            {
                case EKAuthorizationStatus.Authorized:
                    calendarAuthorizationStatusSubject.OnNext(true);
                    return;
                case EKAuthorizationStatus.NotDetermined:
                    requestCalendarAuthorization();
                    return;
                case EKAuthorizationStatus.Denied when force:
                    openAppSettings();
                    return;
                default:
                    calendarAuthorizationStatusSubject.OnNext(false);
                    return;
            }
        }

        private bool isCalendarAuthorized()
            => EKEventStore.GetAuthorizationStatus(EKEntityType.Event) == EKAuthorizationStatus.Authorized;

        private void requestCalendarAuthorization()
        {
            var eventStore = new EKEventStore();
            eventStore.RequestAccess(EKEntityType.Event, (granted, error) =>
            {
                calendarAuthorizationStatusSubject.OnNext(granted);
            });
        }

        #endregion

        public void EnterForeground()
        {
            calendarAuthorizationStatusSubject.OnNext(isCalendarAuthorized());
        }

        private void openAppSettings()
            => UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(UIApplication.OpenSettingsUrlString));
    }
}
