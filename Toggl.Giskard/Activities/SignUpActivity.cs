using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Extensions.Reactive;
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
            this.Bind(ViewModel.ErrorMessage, errorTextView.Rx().TextObserver());
            this.Bind(emailEditText.Rx().Text().Select(Email.From), ViewModel.SetEmail);
            this.Bind(passwordEditText.Rx().Text().Select(Password.From), ViewModel.SetPassword);
            this.Bind(ViewModel.IsLoading.Select(signupButtonTitle), signupButton.Rx().TextObserver());
            this.Bind(ViewModel.CountryButtonTitle, countryNameTextView.Rx().TextObserver());

            //Visibility
            this.Bind(ViewModel.HasError, errorTextView.Rx().IsVisible(useGone: false));
            this.Bind(ViewModel.IsLoading, progressBar.Rx().IsVisible(useGone: false));
            this.Bind(ViewModel.SignupEnabled, signupButton.Rx().Enabled());
            this.Bind(ViewModel.IsCountryErrorVisible, countryErrorView.Rx().IsVisible(useGone: false));

            //Commands
            this.Bind(loginCard.Rx().Tap(), ViewModel.Login);
            this.Bind(signupButton.Rx().Tap(), ViewModel.Signup);
            this.Bind(passwordEditText.Rx().EditorActionSent(), ViewModel.Signup);
            this.BindVoid(googleSignupButton.Rx().Tap(), ViewModel.GoogleSignup);
            this.Bind(countrySelection.Rx().Tap(), ViewModel.PickCountry);

            string signupButtonTitle(bool isLoading)
                => isLoading ? "" : Resources.GetString(Resource.String.SignUpForFree);
        }
    }
}
