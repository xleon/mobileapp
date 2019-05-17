using System;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SiriShortcutsViewModel : ViewModel
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;

        public UIAction NavigateToCustomReportShortcut;
        public UIAction NavigateToCustomTimeEntryShortcut;

        public SiriShortcutsViewModel(
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;

            NavigateToCustomReportShortcut = rxActionFactory.FromAsync(navigateToCustomReportShortcut);
            NavigateToCustomTimeEntryShortcut = rxActionFactory.FromAsync(navigateToCustomTimeEntryShortcut);
        }

        private Task navigateToCustomReportShortcut() => navigationService.Navigate<SiriShortcutsSelectReportPeriodViewModel>();

        private Task navigateToCustomTimeEntryShortcut() =>
            navigationService.Navigate<SiriShortcutsCustomTimeEntryViewModel>();

        public IObservable<IThreadSafeProject> GetProject(long projectId)
            => interactorFactory.GetProjectById(projectId).Execute();
    }
}
