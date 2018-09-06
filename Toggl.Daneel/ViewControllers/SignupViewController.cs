using System;
using System.Linq;
using System.Reactive.Linq;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using UIKit;
using static Toggl.Daneel.Extensions.LoginSignupViewExtensions;
using static Toggl.Daneel.Extensions.UIKitRxExtensions;
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
            this.Bind(ViewModel.Email, EmailTextField.BindText());
            this.Bind(ViewModel.ErrorMessage, ErrorLabel.BindText());
            this.Bind(ViewModel.Password, PasswordTextField.BindText());
            this.Bind(EmailTextField.Text().Select(Email.From), ViewModel.SetEmail);
            this.Bind(PasswordTextField.Text().Select(Password.From), ViewModel.SetPassword);
            this.Bind(ViewModel.IsLoading.Select(signupButtonTitle), SignupButton.BindAnimatedTitle());
            this.Bind(ViewModel.CountryButtonTitle, SelectCountryButton.BindAnimatedTitle());

            //Visibility
            this.Bind(ViewModel.HasError, ErrorLabel.BindAnimatedIsVisible());
            this.Bind(ViewModel.IsLoading, ActivityIndicator.BindIsVisibleWithFade());
            this.Bind(ViewModel.IsPasswordMasked.Skip(1), PasswordTextField.BindSecureTextEntry());
            this.Bind(ViewModel.IsShowPasswordButtonVisible, ShowPasswordButton.BindIsVisible());
            this.Bind(PasswordTextField.FirstResponder, ViewModel.SetIsShowPasswordButtonVisible);
            this.Bind(ViewModel.IsCountryErrorVisible, CountryNotSelectedImageView.BindAnimatedIsVisible());

            //Commands
            this.Bind(LoginCard.Tapped(), ViewModel.Login);
            this.Bind(SignupButton.Tapped(), ViewModel.Signup);
            this.BindVoid(GoogleSignupButton.Tapped(), ViewModel.GoogleSignup);
            this.BindVoid(ShowPasswordButton.Tapped(), ViewModel.TogglePasswordVisibility);
            this.Bind(SelectCountryButton.Tapped(), ViewModel.PickCountry);

            //Color
            this.Bind(ViewModel.HasError.Select(signupButtonTintColor), SignupButton.BindTintColor());
            this.Bind(ViewModel.SignupEnabled.Select(signupButtonTitleColor), SignupButton.BindTitleColor());

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

