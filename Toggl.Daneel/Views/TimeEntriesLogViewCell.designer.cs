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
    [Register ("TimeEntriesLogViewCell")]
    partial class TimeEntriesLogViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ContinueButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DescriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ProjectLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TaskLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TimeLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ContinueButton != null) {
                ContinueButton.Dispose ();
                ContinueButton = null;
            }

            if (DescriptionLabel != null) {
                DescriptionLabel.Dispose ();
                DescriptionLabel = null;
            }

            if (ProjectLabel != null) {
                ProjectLabel.Dispose ();
                ProjectLabel = null;
            }

            if (TaskLabel != null) {
                TaskLabel.Dispose ();
                TaskLabel = null;
            }

            if (TimeLabel != null) {
                TimeLabel.Dispose ();
                TimeLabel = null;
            }
        }
    }
}