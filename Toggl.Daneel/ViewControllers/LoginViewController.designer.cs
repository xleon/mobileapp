// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [Register ("LoginView")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint BottomConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Email { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ForgotPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PasswordManagerButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ShowPassword { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BottomConstraint != null) {
                BottomConstraint.Dispose ();
                BottomConstraint = null;
            }

            if (Email != null) {
                Email.Dispose ();
                Email = null;
            }

            if (ForgotPassword != null) {
                ForgotPassword.Dispose ();
                ForgotPassword = null;
            }

            if (Password != null) {
                Password.Dispose ();
                Password = null;
            }

            if (PasswordManagerButton != null) {
                PasswordManagerButton.Dispose ();
                PasswordManagerButton = null;
            }

            if (ShowPassword != null) {
                ShowPassword.Dispose ();
                ShowPassword = null;
            }
        }
    }
}