using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class OutdatedAppViewModel : ViewModel
    {
        private readonly IRxActionFactory rxActionFactory;

        public UIAction OpenWebsite { get; }

        public UIAction UpdateApp { get; }

        private const string togglWebsiteUrl = "https://toggl.com";

        private readonly IBrowserService browserService;

        public OutdatedAppViewModel(IBrowserService browserService, IRxActionFactory rxActionFactory, INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(browserService, nameof(browserService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.browserService = browserService;
            this.rxActionFactory = rxActionFactory;

            UpdateApp = rxActionFactory.FromAction(updateApp);
            OpenWebsite = rxActionFactory.FromAction(openWebsite);
        }

        private void openWebsite()
        {
            browserService.OpenUrl(togglWebsiteUrl);
        }

        private void updateApp()
        {
            browserService.OpenStore();
        }
    }
}
