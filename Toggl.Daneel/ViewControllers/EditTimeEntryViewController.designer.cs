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
    [Register ("EditViewController")]
    partial class EditTimeEntryViewController
    {
        [Outlet]
        UIKit.UIStackView AddProjectAndTaskView { get; set; }


        [Outlet]
        UIKit.UIStackView AddTagsView { get; set; }


        [Outlet]
        UIKit.UISwitch BillableSwitch { get; set; }


        [Outlet]
        UIKit.UIButton CloseButton { get; set; }


        [Outlet]
        UIKit.UIButton ConfirmButton { get; set; }


        [Outlet]
        UIKit.UIButton DeleteButton { get; set; }


        [Outlet]
        UIKit.UITextView DescriptionTextView { get; set; }


        [Outlet]
        UIKit.UILabel DurationLabel { get; set; }


        [Outlet]
        UIKit.UILabel ProjectTaskClientLabel { get; set; }


        [Outlet]
        UIKit.UILabel StartDateLabel { get; set; }


        [Outlet]
        UIKit.UIStackView StartDateTimeView { get; set; }


        [Outlet]
        UIKit.UILabel StartTimeLabel { get; set; }


        [Outlet]
        UIKit.UILabel TagsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BillableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ScrollViewContent { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AddProjectAndTaskView != null) {
                AddProjectAndTaskView.Dispose ();
                AddProjectAndTaskView = null;
            }

            if (AddTagsView != null) {
                AddTagsView.Dispose ();
                AddTagsView = null;
            }

            if (BillableSwitch != null) {
                BillableSwitch.Dispose ();
                BillableSwitch = null;
            }

            if (BillableView != null) {
                BillableView.Dispose ();
                BillableView = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (ConfirmButton != null) {
                ConfirmButton.Dispose ();
                ConfirmButton = null;
            }

            if (DeleteButton != null) {
                DeleteButton.Dispose ();
                DeleteButton = null;
            }

            if (DescriptionTextView != null) {
                DescriptionTextView.Dispose ();
                DescriptionTextView = null;
            }

            if (DurationLabel != null) {
                DurationLabel.Dispose ();
                DurationLabel = null;
            }

            if (ProjectTaskClientLabel != null) {
                ProjectTaskClientLabel.Dispose ();
                ProjectTaskClientLabel = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (ScrollViewContent != null) {
                ScrollViewContent.Dispose ();
                ScrollViewContent = null;
            }

            if (StartDateLabel != null) {
                StartDateLabel.Dispose ();
                StartDateLabel = null;
            }

            if (StartDateTimeView != null) {
                StartDateTimeView.Dispose ();
                StartDateTimeView = null;
            }

            if (StartTimeLabel != null) {
                StartTimeLabel.Dispose ();
                StartTimeLabel = null;
            }

            if (TagsLabel != null) {
                TagsLabel.Dispose ();
                TagsLabel = null;
            }
        }
    }
}