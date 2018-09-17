using System.Linq;
using System.Reactive.Linq;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using UIKit;
using static Toggl.Daneel.Extensions.LoginSignupViewExtensions;
using static Toggl.Daneel.Extensions.ViewExtensions;


namespace Toggl.Daneel.ViewControllers
{
    [MvxRootPresentation(WrapInNavigationController = true)]
    public sealed partial class SignupViewController : ReactiveViewController<SignupViewModel>
    {
        private const int iPhoneSeScreenHeight = 568;

        private bool keyboardIsOpen = false;

        private const int topConstraintForBiggerScreens = 72;
        private const int topConstraintForBiggerScreensWithKeyboard = 40;

        private const int emailTopConstraint = 42;
        private const int emailTopConstraintWithKeyboard = 12;

        public SignupViewController() : base(nameof(SignupViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBarHidden = true;

            UIKeyboard.Notifications.ObserveWillShow(KeyboardWillShow);
            UIKeyboard.Notifications.ObserveWillHide(KeyboardWillHide);

            //Text
            this.Bind(ViewModel.ErrorMessage, ErrorLabel.Rx().Text());
            this.Bind(ViewModel.Email, EmailTextField.Rx().TextObserver());
            this.Bind(ViewModel.Password, PasswordTextField.Rx().TextObserver());
            this.Bind(EmailTextField.Rx().Text().Select(Email.From), ViewModel.SetEmail);
            this.Bind(PasswordTextField.Rx().Text().Select(Password.From), ViewModel.SetPassword);
            this.Bind(ViewModel.IsLoading.Select(signupButtonTitle), SignupButton.Rx().AnimatedTitle());
            this.Bind(ViewModel.CountryButtonTitle, SelectCountryButton.Rx().AnimatedTitle());

            //Visibility
            this.Bind(ViewModel.HasError, ErrorLabel.Rx().AnimatedIsVisible());
            this.Bind(ViewModel.IsLoading, ActivityIndicator.Rx().IsVisibleWithFade());
            this.Bind(ViewModel.IsPasswordMasked.Skip(1), PasswordTextField.Rx().SecureTextEntry());
            this.Bind(ViewModel.IsShowPasswordButtonVisible, ShowPasswordButton.Rx().IsVisible());
            this.Bind(PasswordTextField.FirstResponder, ViewModel.SetIsShowPasswordButtonVisible);
            this.Bind(ViewModel.IsCountryErrorVisible, CountryNotSelectedImageView.Rx().AnimatedIsVisible());

            //Commands
            this.Bind(LoginCard.Rx().Tap(), ViewModel.Login);
            this.Bind(SignupButton.Rx().Tap(), ViewModel.Signup);
            this.BindVoid(GoogleSignupButton.Rx().Tap(), ViewModel.GoogleSignup);
            this.BindVoid(ShowPasswordButton.Rx().Tap(), ViewModel.TogglePasswordVisibility);
            this.Bind(SelectCountryButton.Rx().Tap(), ViewModel.PickCountry);

            //Color
            this.Bind(ViewModel.HasError.Select(signupButtonTintColor), SignupButton.Rx().TintColor());
            this.Bind(ViewModel.SignupEnabled.Select(signupButtonTitleColor), SignupButton.Rx().TitleColor());

            //Animation
            this.Bind(ViewModel.Shake, shakeTargets =>
            {
                if (shakeTargets.HasFlag(SignupViewModel.ShakeTargets.Email))
                    EmailTextField.Shake();

                if (shakeTargets.HasFlag(SignupViewModel.ShakeTargets.Password))
                    PasswordTextField.Shake();

                if (shakeTargets.HasFlag(SignupViewModel.ShakeTargets.Country))
                    SelectCountryButton.Shake();
            });

            prepareViews();

            UIColor signupButtonTintColor(bool hasError)
                => hasError ? UIColor.White : UIColor.Black;

            UIColor signupButtonTitleColor(bool enabled) => enabled
                ? Color.Login.EnabledButtonColor.ToNativeColor()
                : Color.Login.DisabledButtonColor.ToNativeColor();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (View.Frame.Height > iPhoneSeScreenHeight && !keyboardIsOpen)
                TopConstraint.Constant = topConstraintForBiggerScreens;

            LoginCard.SetupBottomCard();
            GoogleSignupButton.SetupGoogleButton();
        }

        private void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            keyboardIsOpen = true;
            if (View.Frame.Height > iPhoneSeScreenHeight)
            {
                TopConstraint.Constant = topConstraintForBiggerScreensWithKeyboard;
            }
            else
            {
                EmailFieldTopConstraint.Constant = emailTopConstraintWithKeyboard;
            }
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        private void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            keyboardIsOpen = false;
            if (View.Frame.Height > iPhoneSeScreenHeight)
            {
                TopConstraint.Constant = topConstraintForBiggerScreens;
            }
            else
            {
                EmailFieldTopConstraint.Constant = emailTopConstraint;
            }
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        private void prepareViews()
        {
            NavigationController.NavigationBarHidden = true;

            ActivityIndicator.Alpha = 0;
            ActivityIndicator.StartSpinning();

            SignupButton.SetTitleColor(
                Color.Login.DisabledButtonColor.ToNativeColor(),
                UIControlState.Disabled
            );

            EmailTextField.ShouldReturn += _ => {
                PasswordTextField.BecomeFirstResponder();
                return false;
            };

            PasswordTextField.ShouldReturn += _ =>
            {
                ViewModel.Signup();
                PasswordTextField.ResignFirstResponder();
                return false;
            };

            View.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                EmailTextField.ResignFirstResponder();
                PasswordTextField.ResignFirstResponder();
            }));

            ShowPasswordButton.SetupShowPasswordButton();
        }

        private string signupButtonTitle(bool isLoading)
            => isLoading ? "" : Resources.SignUpTitle;
    }
}

