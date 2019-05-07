using Android.Support.Design.Widget;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Toggl.Droid.Activities
{
    public sealed partial class TokenResetActivity
    {
        private Toolbar toolbar;

        private EditText passwordEditText;
        private ProgressBar progressBar;
        private TextView emailLabel;
        private TextView signoutLabel;
        private FloatingActionButton doneButton;

        protected override void InitializeViews()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);

            passwordEditText = FindViewById<EditText>(Resource.Id.TokenResetPassword);
            progressBar = FindViewById<ProgressBar>(Resource.Id.TokenResetProgressBar);
            emailLabel = FindViewById<TextView>(Resource.Id.TokenResetEmailLabel);
            signoutLabel = FindViewById<TextView>(Resource.Id.TokenResetSignOutLabel);
            doneButton = FindViewById<FloatingActionButton>(Resource.Id.TokenResetDoneButton);
        }
    }
}
