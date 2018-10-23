using System;
using System.Reactive;
using System.Reactive.Linq;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TermsOfServiceViewModel : MvxViewModelResult<bool>
    {
        private const string privacyPolicyUrl = "https://toggl.com/legal/privacy/";
        private const string termsOfServiceUrl = "https://toggl.com/legal/terms/";

        private readonly IBrowserService browserService;
        private readonly IMvxNavigationService navigationService;

        public UIAction ViewTermsOfService { get; }

        public UIAction ViewPrivacyPolicy { get; }

        public UIAction Close { get; }

        public UIAction Accept { get; }

        public TermsOfServiceViewModel(
            IBrowserService browserService, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(browserService, nameof(browserService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;
            this.browserService = browserService;

            ViewPrivacyPolicy = new UIAction(() => openUrl(privacyPolicyUrl));
            ViewTermsOfService = new UIAction(() => openUrl(termsOfServiceUrl));

            Close = new UIAction(() => close(false));
            Accept = new UIAction(() => close(true));
        }

        private IObservable<Unit> close(bool isAccepted)
            => navigationService.Close(this, isAccepted)
                .ToUnitObservable();

        private IObservable<Unit> openUrl(string url)
        {
            browserService.OpenUrl(url);
            return Observable.Return(Unit.Default);
        }
    }
}
