using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
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
    public sealed partial class LoginActivity : ReactiveActivity<LoginViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out);

            InitializeViews();

            emailEditText.Text = ViewModel.Email.FirstAsync().GetAwaiter().GetResult();
            passwordEditText.Text = ViewModel.Password.FirstAsync().GetAwaiter().GetResult();

            //Text
            this.Bind(ViewModel.ErrorMessage, errorTextView.Rx().TextObserver());
            this.Bind(emailEditText.Rx().Text().Select(Email.From), ViewModel.SetEmail);
            this.Bind(passwordEditText.Rx().Text().Select(Password.From), ViewModel.SetPassword);
            this.Bind(ViewModel.IsLoading.Select(loginButtonTitle), loginButton.Rx().TextObserver());

            //Visibility
            this.Bind(ViewModel.HasError, errorTextView.Rx().IsVisible(useGone: false));
            this.Bind(ViewModel.IsLoading, progressBar.Rx().IsVisible(useGone: false));
            this.Bind(ViewModel.LoginEnabled, loginButton.Rx().Enabled());

            //Commands
            this.Bind(signupCard.Rx().Tap(), ViewModel.Signup);
            this.BindVoid(loginButton.Rx().Tap(), ViewModel.Login);
            this.BindVoid(passwordEditText.Rx().EditorActionSent(), ViewModel.Login);
            this.BindVoid(googleLoginButton.Rx().Tap(), ViewModel.GoogleLogin);
            this.Bind(forgotPasswordView.Rx().Tap(), ViewModel.ForgotPassword);

            string loginButtonTitle(bool isLoading)
                => isLoading ? "" : Resources.GetString(Resource.String.Login);

            this.CancelAllNotifications();
        }
    }
}
