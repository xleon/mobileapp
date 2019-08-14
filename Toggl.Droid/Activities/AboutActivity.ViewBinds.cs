using Android.Views;

namespace Toggl.Droid.Activities
{
    public partial class AboutActivity
    {
        private View licensesButton;
        private View privacyPolicyButton;
        private View termsOfServiceButton;

        protected override void InitializeViews()
        {
            licensesButton = FindViewById(Resource.Id.LicensesButton);
            privacyPolicyButton = FindViewById(Resource.Id.PrivacyPolicyButton);
            termsOfServiceButton = FindViewById(Resource.Id.TermsOfServiceButton);
        }
    }
}
