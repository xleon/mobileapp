using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Extensions;
using Toggl.Multivac;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme.WhiteStatusBar",
              ScreenOrientation = ScreenOrientation.Portrait,
              WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class SignUpActivity : ReactiveActivity<SignupViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            this.ChangeStatusBarColor(Color.White, true);

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SignUpActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out);

            InitializeViews();

            emailEditText.Text = ViewModel.Email.FirstAsync().GetAwaiter().GetResult();
            passwordEditText.Text = ViewModel.Password.FirstAsync().GetAwaiter().GetResult();

            //Text
            this.Bind(ViewModel.ErrorMessage, errorTextView.BindText());
            this.Bind(emailEditText.Text().Select(Email.From), ViewModel.SetEmail);
            this.Bind(passwordEditText.Text().Select(Password.From), ViewModel.SetPassword);
            this.Bind(ViewModel.IsLoading.Select(signupButtonTitle), signupButton.BindText());
            this.Bind(ViewModel.CountryButtonTitle, countryNameTextView.BindText());

            //Visibility
            this.Bind(ViewModel.HasError, errorTextView.BindIsVisible(useGone: false));
            this.Bind(ViewModel.IsLoading, progressBar.BindIsVisible(useGone: false));
            this.Bind(ViewModel.SignupEnabled, signupButton.BindEnabled());
            this.Bind(ViewModel.IsCountryErrorVisible, countryErrorView.BindIsVisible(useGone: false));

            //Commands
            this.Bind(loginCard.Tapped(), ViewModel.Login);
            this.Bind(signupButton.Tapped(), ViewModel.Signup);
            this.Bind(passwordEditText.EditorActionSent(), ViewModel.Signup);
            this.BindVoid(googleSignupButton.Tapped(), ViewModel.GoogleSignup);
            this.Bind(countrySelection.Tapped(), ViewModel.PickCountry);

            string signupButtonTitle(bool isLoading)
                => isLoading ? "" : Resources.GetString(Resource.String.SignUpForFree);
        }
    }
}
