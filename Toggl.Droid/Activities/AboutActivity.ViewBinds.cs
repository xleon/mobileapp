using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Activities
{
    public partial class AboutActivity
    {
        private TextView licensesButton;
        private TextView privacyPolicyButton;
        private TextView termsOfServiceButton;

        protected override void InitializeViews()
        {
            licensesButton = FindViewById(Resource.Id.LicensesButton);
            privacyPolicyButton = FindViewById(Resource.Id.PrivacyPolicyButton);
            termsOfServiceButton = FindViewById(Resource.Id.TermsOfServiceButton);
            
            licensesButton.Text = Shared.Resources.Licenses;
            privacyPolicyButton.Text = Shared.Resources.PrivacyPolicy;
            termsOfServiceButton.Text = Shared.Resources.TermsOfService;

            SetupToolbar(title: Shared.Resources.About);
        }
    }
}
