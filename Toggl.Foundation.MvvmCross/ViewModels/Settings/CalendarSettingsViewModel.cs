using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels.Settings
{
    public sealed class CalendarSettingsViewModel : SelectUserCalendarsViewModelBase
    {
        private readonly IPermissionsService permissionsService;

        public bool PermissionGranted { get; }

        public UIAction RequestAccessAction { get; }

        public CalendarSettingsViewModel(
            IInteractorFactory interactorFactory,
            IPermissionsService permissionsService) : base(interactorFactory)
        {
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));

            this.permissionsService = permissionsService;

            PermissionGranted = permissionsService.CalendarPermissionGranted;

            RequestAccessAction = new UIAction(requestAccess);
        }

        private IObservable<Unit> requestAccess()
        {
            permissionsService.OpenAppSettings();
            return Observable.Return(Unit.Default);
        }
    }
}
