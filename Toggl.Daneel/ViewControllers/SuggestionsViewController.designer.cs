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
    [Register ("SuggestionsViewController")]
    partial class SuggestionsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView SuggestionsTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel WelcomeTextLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel WelcomeTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView WelcomeView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SuggestionsTableView != null) {
                SuggestionsTableView.Dispose ();
                SuggestionsTableView = null;
            }

            if (WelcomeTextLabel != null) {
                WelcomeTextLabel.Dispose ();
                WelcomeTextLabel = null;
            }

            if (WelcomeTitleLabel != null) {
                WelcomeTitleLabel.Dispose ();
                WelcomeTitleLabel = null;
            }

            if (WelcomeView != null) {
                WelcomeView.Dispose ();
                WelcomeView = null;
            }
        }
    }
}