using Android.Support.Design.Widget;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Toggl.Droid.Activities
{
    public sealed partial class TokenResetActivity
    {
        private Toolbar toolbar;

        private TextInputLayout tokenResetPasswordLayout;
        private EditText passwordEditText;
        private ProgressBar progressBar;
        private TextView tokenResetMessageWarning;
        private TextView tokenResetMessageEnterPasswordLabel;
        private TextView emailLabel;
        private TextView signoutLabel;
        private FloatingActionButton doneButton;

        protected override void InitializeViews()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);

            tokenResetPasswordLayout = FindViewById<TextInputLayout>(Resource.Id.TokenResetPasswordLayout);
            passwordEditText = FindViewById<EditText>(Resource.Id.TokenResetPassword);
            progressBar = FindViewById<ProgressBar>(Resource.Id.TokenResetProgressBar);
            tokenResetMessageWarning = FindViewById<TextView>(Resource.Id.TokenResetMessageWarning);
            tokenResetMessageEnterPasswordLabel = FindViewById<TextView>(Resource.Id.TokenResetMessageEnterPasswordLabel);
            emailLabel = FindViewById<TextView>(Resource.Id.TokenResetEmailLabel);
            signoutLabel = FindViewById<TextView>(Resource.Id.TokenResetSignOutLabel);
            doneButton = FindViewById<FloatingActionButton>(Resource.Id.TokenResetDoneButton);
        }
    }
}
