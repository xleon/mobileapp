using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Core.UI.Parameters;
using Toggl.Shared.Extensions;
using Toggl.Core.Services;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class AboutViewModel : ViewModel
    {
        private readonly INavigationService navigationService;
        private readonly IRxActionFactory rxActionFactory;

        public UIAction OpenPrivacyPolicyView { get; private set; }
        public UIAction OpenTermsOfServiceView { get; private set; }
        public UIAction OpenLicensesView { get; private set; }

        public AboutViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;
            this.rxActionFactory = rxActionFactory;

            OpenPrivacyPolicyView = rxActionFactory.FromAsync(openPrivacyPolicyView);
            OpenTermsOfServiceView = rxActionFactory.FromAsync(openTermsOfServiceView);
            OpenLicensesView = rxActionFactory.FromAsync(openLicensesView);
        }

        private Task openPrivacyPolicyView()
            => navigationService
                .Navigate<BrowserViewModel, BrowserParameters>(
                    BrowserParameters.WithUrlAndTitle(Resources.PrivacyPolicyUrl, Resources.PrivacyPolicy));

        private Task openTermsOfServiceView()
            => navigationService
                .Navigate<BrowserViewModel, BrowserParameters>(
                    BrowserParameters.WithUrlAndTitle(Resources.TermsOfServiceUrl, Resources.TermsOfService));

        private Task openLicensesView()
            => navigationService
                .Navigate<LicensesViewModel>();
    }
}
