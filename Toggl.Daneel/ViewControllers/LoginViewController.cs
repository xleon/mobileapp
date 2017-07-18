using CoreText;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public partial class LoginViewController : MvxViewController<LoginViewModel>
    {
        private const int ForgotPasswordLabelOffset = 27;

        public LoginViewController() 
            : base(nameof(LoginViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
  
            Title = ViewModel.Title;

            prepareTextFields();

            UIKeyboard.Notifications.ObserveWillShow(keyboardWillShow);
            UIKeyboard.Notifications.ObserveWillHide(keyboardWillHide);

            NavigationItem.LeftBarButtonItem = 
                new UIBarButtonItem { Title = Resources.LoginBackButton, TintColor = UIColor.White };

            NavigationItem.RightBarButtonItem =
                new UIBarButtonItem { Title = Resources.LoginNextButton, TintColor = UIColor.White };
            
            var bindingSet = this.CreateBindingSet<LoginViewController, LoginViewModel>();

            //Text
            bindingSet.Bind(Email).To(vm => vm.Email);
            bindingSet.Bind(Password).To(vm => vm.Password);
            bindingSet.Bind(Password)
                      .For(v => v.BindSecureTextEntry())
                      .To(vm => vm.IsPasswordMasked);

            //Commands
            bindingSet.Bind(NavigationItem.LeftBarButtonItem)
                      .For(v => v.BindCommand())
                      .To(vm => vm.BackCommand);

            bindingSet.Bind(NavigationItem.RightBarButtonItem)
                      .For(v => v.BindCommand())
                      .To(vm => vm.NextCommand);

            bindingSet.Bind(ShowPassword)
                      .For(v => v.BindTap())
                      .To(vm => vm.TogglePasswordVisibilityCommand);

            bindingSet.Bind(PasswordManagerButton)
                      .For(v => v.BindTap())
                      .To(vm => vm.StartPasswordManagerCommand);

            //Enabled
            bindingSet.Bind(NavigationItem.RightBarButtonItem)
                      .For(v => v.BindAnimatedEnabled())
                      .To(vm => vm.NextIsEnabled);

            //Visibility
            bindingSet.Bind(Email)
                      .For(v => v.BindAnimatedVisibility())
                      .To(vm => vm.IsEmailPage);

            bindingSet.Bind(Password)
                      .For(v => v.BindAnimatedVisibility())
                      .To(vm => vm.IsPasswordPage);

            bindingSet.Bind(ShowPassword)
                      .For(v => v.BindAnimatedVisibility())
                      .To(vm => vm.IsPasswordPage);

            if (ViewModel.IsPasswordManagerAvailable)
            {
                bindingSet.Bind(PasswordManagerButton)
                          .For(v => v.BindAnimatedVisibility())
                          .To(vm => vm.IsEmailPage);
            }
            else
            {
                PasswordManagerButton.Hidden = true;
            }

            //State
            bindingSet.Bind(Email)
                      .For(v => v.BindFocus())
                      .To(vm => vm.IsEmailPage);
            
            bindingSet.Bind(Password)
                      .For(v => v.BindFocus())
                      .To(vm => vm.IsPasswordPage);
            
            bindingSet.Apply();

            Email.BecomeFirstResponder();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBar.UserInteractionEnabled = true;
        }

        private void keyboardWillShow(object sender, UIKeyboardEventArgs e)
            => BottomConstraint.Constant = e.FrameBegin.Height + ForgotPasswordLabelOffset;

        private void keyboardWillHide(object sender, UIKeyboardEventArgs e)
            => BottomConstraint.Constant = ForgotPasswordLabelOffset;

        private void prepareTextFields()
        {
            var stringAttributes = new CTStringAttributes(
                new UIStringAttributes { ForegroundColor = UIColor.White.ColorWithAlpha(0.5f) }.Dictionary
            );

            Email.AttributedPlaceholder = 
                new NSAttributedString(Resources.LoginSignUpEmailPlaceholder, stringAttributes);

            Password.AttributedPlaceholder = 
                new NSAttributedString(Resources.LoginSignUpPasswordPlaceholder, stringAttributes);

            ForgotPassword.SetTitle(Resources.LoginForgotPassword, UIControlState.Normal);
        }
    }
}

