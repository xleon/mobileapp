// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.Views
{
    [Register ("SuggestionsViewCell")]
    partial class SuggestionsViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BottomRowView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DescriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ProjectDot { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ProjectLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StartButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TaskLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BottomRowView != null) {
                BottomRowView.Dispose ();
                BottomRowView = null;
            }

            if (DescriptionLabel != null) {
                DescriptionLabel.Dispose ();
                DescriptionLabel = null;
            }

            if (ProjectDot != null) {
                ProjectDot.Dispose ();
                ProjectDot = null;
            }

            if (ProjectLabel != null) {
                ProjectLabel.Dispose ();
                ProjectLabel = null;
            }

            if (StartButton != null) {
                StartButton.Dispose ();
                StartButton = null;
            }

            if (TaskLabel != null) {
                TaskLabel.Dispose ();
                TaskLabel = null;
            }
        }
    }
}