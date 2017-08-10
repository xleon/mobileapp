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
        UIKit.UIView CurrentTimeEntryCard { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CurrentTimeEntryDescriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CurrentTimeEntryElapsedTimeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EditTimeEntryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StartTimeEntryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StopTimeEntryButton { get; set; }

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
            if (CurrentTimeEntryCard != null) {
                CurrentTimeEntryCard.Dispose ();
                CurrentTimeEntryCard = null;
            }

            if (CurrentTimeEntryDescriptionLabel != null) {
                CurrentTimeEntryDescriptionLabel.Dispose ();
                CurrentTimeEntryDescriptionLabel = null;
            }

            if (CurrentTimeEntryElapsedTimeLabel != null) {
                CurrentTimeEntryElapsedTimeLabel.Dispose ();
                CurrentTimeEntryElapsedTimeLabel = null;
            }

            if (EditTimeEntryButton != null) {
                EditTimeEntryButton.Dispose ();
                EditTimeEntryButton = null;
            }

            if (StartTimeEntryButton != null) {
                StartTimeEntryButton.Dispose ();
                StartTimeEntryButton = null;
            }

            if (StopTimeEntryButton != null) {
                StopTimeEntryButton.Dispose ();
                StopTimeEntryButton = null;
            }

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