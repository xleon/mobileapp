using MvvmCross.Navigation;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class SelectUserCalendarsViewModel : SelectUserCalendarsViewModelBase
    {
        public SelectUserCalendarsViewModel(
            IUserPreferences userPreferences,
            IInteractorFactory interactorFactory,
            IMvxNavigationService navigationService,
            IRxActionFactory rxActionFactory
            )
            : base(userPreferences, interactorFactory, navigationService, rxActionFactory)
        {
        }
    }
}
