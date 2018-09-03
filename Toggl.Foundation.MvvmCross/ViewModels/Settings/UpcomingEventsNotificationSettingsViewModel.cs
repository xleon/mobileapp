using MvvmCross.Navigation;
using Toggl.Foundation.MvvmCross.Helper;

namespace Toggl.Foundation.MvvmCross.ViewModels.Settings
{
    public sealed class UpcomingEventsNotificationSettingsViewModel : SelectFromListViewModel<UpcomingEventsOption>
    {
        public UpcomingEventsNotificationSettingsViewModel(IMvxNavigationService navigationService)
            : base(navigationService) { }
    }
}
