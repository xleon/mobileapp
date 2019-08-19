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
            licensesButton = FindViewById<TextView>(Resource.Id.AboutLicensesButton);
            privacyPolicyButton = FindViewById<TextView>(Resource.Id.AboutPrivacyPolicyButton);
            termsOfServiceButton = FindViewById<TextView>(Resource.Id.AboutTermsOfServiceButton);
        }
    }
}
