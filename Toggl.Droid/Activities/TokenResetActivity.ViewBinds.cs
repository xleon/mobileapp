using Android.Support.Design.Widget;
using Android.Widget;

namespace Toggl.Droid.Activities
{
    public sealed partial class TokenResetActivity
    {
        private EditText passwordEditText;
        private ProgressBar progressBar;
        private TextView emailLabel;
        private TextView signoutLabel;
        private FloatingActionButton doneButton;

        protected override void InitializeViews()
        {
            passwordEditText = FindViewById<EditText>(Resource.Id.TokenResetPassword);
            progressBar = FindViewById<ProgressBar>(Resource.Id.TokenResetProgressBar);
            emailLabel = FindViewById<TextView>(Resource.Id.TokenResetEmailLabel);
            signoutLabel = FindViewById<TextView>(Resource.Id.TokenResetSignOutLabel);
            doneButton = FindViewById<FloatingActionButton>(Resource.Id.TokenResetDoneButton);

            SetupToolbar(Shared.Resources.LoginTitle, showHomeAsUp: false);
        }
    }
}
