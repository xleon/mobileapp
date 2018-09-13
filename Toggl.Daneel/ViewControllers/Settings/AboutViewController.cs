using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Daneel.ViewControllers
{
    public sealed partial class AboutViewController : ReactiveViewController<AboutViewModel>
    {
        public AboutViewController() 
            : base(nameof(AboutViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.About;

            this.Bind(LicensesView.Rx().Tap(), ViewModel.OpenLicensesView);
            this.Bind(PrivacyPolicyView.Rx().Tap(), ViewModel.OpenPrivacyPolicyView);
            this.Bind(TermsOfServiceView.Rx().Tap(), ViewModel.OpenTermsOfServiceView);
        }
    }
}
