using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public sealed partial class TermsOfServiceFragment
    {
        private TextView reviewTheTermsTextView;
        private TextView termsMessageTextView;
        private TextView termsOfServiceTextView;
        private TextView andTextView;
        private TextView privacyPolicyTextView;
        private Button acceptButton;

        protected override void InitializeViews(View fragmentView)
        {
            reviewTheTermsTextView = fragmentView.FindViewById<TextView>(Resource.Id.ReviewTheTermsTextView);
            termsMessageTextView = fragmentView.FindViewById<TextView>(Resource.Id.TermsMessageTextView);
            termsOfServiceTextView = fragmentView.FindViewById<TextView>(Resource.Id.ViewTermsOfServiceTextView);
            andTextView = fragmentView.FindViewById<TextView>(Resource.Id.AndTextView);
            privacyPolicyTextView = fragmentView.FindViewById<TextView>(Resource.Id.ViewPrivacyPolicyTextView);
            acceptButton = fragmentView.FindViewById<Button>(Resource.Id.AcceptButton);
        }
    }
}
