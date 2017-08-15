// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.ViewControllers
{
	[Register ("EditViewController")]
	partial class EditTimeEntryViewController
	{
		[Outlet]
		UIKit.UIStackView AddDescriptionView { get; set; }

		[Outlet]
		UIKit.UIStackView AddProjectAndTaskView { get; set; }

		[Outlet]
		UIKit.UIStackView AddTagsView { get; set; }

		[Outlet]
		UIKit.UISwitch BillableSwitch { get; set; }

		[Outlet]
		UIKit.UILabel ClientLabel { get; set; }

		[Outlet]
		UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		UIKit.UIButton ConfirmButton { get; set; }

		[Outlet]
		UIKit.UIButton DeleteButton { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		UIKit.UIView ProjectDot { get; set; }

		[Outlet]
		UIKit.UILabel ProjectLabel { get; set; }

		[Outlet]
		UIKit.UILabel StartDateLabel { get; set; }

		[Outlet]
		UIKit.UILabel StartTimeLabel { get; set; }

		[Outlet]
		UIKit.UILabel TagsLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ClientLabel != null) {
				ClientLabel.Dispose ();
				ClientLabel = null;
			}

			if (AddDescriptionView != null) {
				AddDescriptionView.Dispose ();
				AddDescriptionView = null;
			}

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

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DurationLabel != null) {
				DurationLabel.Dispose ();
				DurationLabel = null;
			}

			if (ProjectDot != null) {
				ProjectDot.Dispose ();
				ProjectDot = null;
			}

			if (ProjectLabel != null) {
				ProjectLabel.Dispose ();
				ProjectLabel = null;
			}

			if (StartDateLabel != null) {
				StartDateLabel.Dispose ();
				StartDateLabel = null;
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
