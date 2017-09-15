// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.ViewControllers
{
    [Register ("EditDurationViewController")]
    partial class EditDurationViewController
    {
        [Outlet]
        UIKit.UIButton CloseButton { get; set; }


        [Outlet]
        UIKit.UILabel DurationLabel { get; set; }


        [Outlet]
        Toggl.Daneel.Views.ResizableView DurationView { get; set; }


        [Outlet]
        UIKit.UILabel EndTimeLabel { get; set; }


        [Outlet]
        UIKit.UIButton SaveButton { get; set; }


        [Outlet]
        UIKit.UILabel StartTimeLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (DurationLabel != null) {
                DurationLabel.Dispose ();
                DurationLabel = null;
            }

            if (DurationView != null) {
                DurationView.Dispose ();
                DurationView = null;
            }

            if (EndTimeLabel != null) {
                EndTimeLabel.Dispose ();
                EndTimeLabel = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }

            if (StartTimeLabel != null) {
                StartTimeLabel.Dispose ();
                StartTimeLabel = null;
            }
        }
    }
}