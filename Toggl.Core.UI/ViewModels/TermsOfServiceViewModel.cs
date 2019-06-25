using Toggl.Core.UI.Navigation;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TermsOfServiceViewModel : ViewModelWithOutput<bool>
    {
        private const string privacyPolicyUrl = "https://toggl.com/legal/privacy/";
        private const string termsOfServiceUrl = "https://toggl.com/legal/terms/";

        public UIAction ViewTermsOfService { get; }
        public UIAction ViewPrivacyPolicy { get; }

        public TermsOfServiceViewModel(IRxActionFactory rxActionFactory, INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            ViewPrivacyPolicy = rxActionFactory.FromAsync(openPrivacyPolicy);
            ViewTermsOfService = rxActionFactory.FromAsync(openTermsOfService);
        }

        private Task openPrivacyPolicy()
            => Browser.OpenAsync(privacyPolicyUrl, BrowserLaunchMode.External);

        private Task openTermsOfService()
            => Browser.OpenAsync(termsOfServiceUrl, BrowserLaunchMode.External);
    }
}
