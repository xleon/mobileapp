using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Foundation.MvvmCross.ViewModels.Settings
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarSettingsViewModel : SelectUserCalendarsViewModelBase
    {
        private readonly IPermissionsService permissionsService;
        private readonly IUserPreferences userPreferences;

        public bool PermissionGranted { get; }

        public UIAction RequestAccessAction { get; }

        public CalendarSettingsViewModel(
            IUserPreferences userPreferences,
            IInteractorFactory interactorFactory,
            IPermissionsService permissionsService) : base(interactorFactory)
        {
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));

            this.userPreferences = userPreferences;
            this.permissionsService = permissionsService;

            PermissionGranted = permissionsService.CalendarPermissionGranted;

            RequestAccessAction = new UIAction(requestAccess);

            SelectCalendarAction
                .Elements
                .VoidSubscribe(onCalendarSelected);
        }

        public override Task Initialize()
        {
            SelectedCalendarIds.AddRange(userPreferences.EnabledCalendarIds());
            return base.Initialize();
        }

        private IObservable<Unit> requestAccess()
        {
            permissionsService.OpenAppSettings();
            return Observable.Return(Unit.Default);
        }

        private void onCalendarSelected()
        {
            userPreferences.SetEnabledCalendars(SelectedCalendarIds.ToArray());
        }
    }
}
