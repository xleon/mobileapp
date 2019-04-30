using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TermsOfServiceViewModel : ViewModelWithOutput<bool>
    {
        private readonly IRxActionFactory rxActionFactory;

        private const string privacyPolicyUrl = "https://toggl.com/legal/privacy/";
        private const string termsOfServiceUrl = "https://toggl.com/legal/terms/";

        private readonly IBrowserService browserService;

        public UIAction ViewTermsOfService { get; }
        public UIAction ViewPrivacyPolicy { get; }
        public InputAction<bool> Close { get; }

        public TermsOfServiceViewModel(IBrowserService browserService, IRxActionFactory rxActionFactory, INavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(browserService, nameof(browserService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.browserService = browserService;
            this.rxActionFactory = rxActionFactory;

            ViewPrivacyPolicy = rxActionFactory.FromAction(() => openUrl(privacyPolicyUrl));
            ViewTermsOfService = rxActionFactory.FromAction(() => openUrl(termsOfServiceUrl));
            Close = rxActionFactory.FromAsync<bool>(result => Finish(result));
        }

        private void openUrl(string url)
        {
            browserService.OpenUrl(url);
        }
    }
}
