using System;
using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Activities
{
    public sealed partial class SignUpActivity
    {
        private Button signupButton;

        private View loginCard;
        private View googleSignupButton;

        private ImageView countryErrorView;

        private EditText emailEditText;
        private EditText passwordEditText;

        private TextView errorTextView;
        private TextView countryNameTextView;

        private LinearLayout countrySelection;

        private ProgressBar progressBar;

        protected override void InitializeViews()
        {
            loginCard = FindViewById(Resource.Id.LoginSignupCardView);
            errorTextView = FindViewById<TextView>(Resource.Id.SignUpError);
            countryNameTextView = FindViewById<TextView>(Resource.Id.SignUpCountryName);
            countrySelection = FindViewById<LinearLayout>(Resource.Id.SignUpCountrySelection);
            signupButton = FindViewById<Button>(Resource.Id.SignUpButton);
            googleSignupButton = FindViewById<View>(Resource.Id.SignUpWithGoogleButton);
            progressBar = FindViewById<ProgressBar>(Resource.Id.SignUpProgressBar);
            emailEditText = FindViewById<EditText>(Resource.Id.SignUpEmailEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.SignUpPasswordEditText);
            countryErrorView = FindViewById<ImageView>(Resource.Id.SignUpCountryErrorView);
        }
    }
}