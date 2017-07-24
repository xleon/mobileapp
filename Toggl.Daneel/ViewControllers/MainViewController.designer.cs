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
    [Register ("MainViewController")]
    partial class MainViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SuggestionsContainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TimeEntriesLogContainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView TimelineView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SuggestionsContainer != null) {
                SuggestionsContainer.Dispose ();
                SuggestionsContainer = null;
            }

            if (TimeEntriesLogContainer != null) {
                TimeEntriesLogContainer.Dispose ();
                TimeEntriesLogContainer = null;
            }

            if (TimelineView != null) {
                TimelineView.Dispose ();
                TimelineView = null;
            }
        }
    }
}