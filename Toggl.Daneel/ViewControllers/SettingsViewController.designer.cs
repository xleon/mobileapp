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
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		UIKit.UIImageView LoggingOutIndicator { get; set; }

		[Outlet]
		UIKit.UILabel LoggingOutLabel { get; set; }

		[Outlet]
		UIKit.UIView LoggingOutView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton LogoutButton { get; set; }

		[Outlet]
		UIKit.UIImageView SyncedIcon { get; set; }

		[Outlet]
		UIKit.UILabel SyncedLabel { get; set; }

		[Outlet]
		UIKit.UIView SyncedView { get; set; }

		[Outlet]
		UIKit.UIImageView SyncingIndicator { get; set; }

		[Outlet]
		UIKit.UILabel SyncingLabel { get; set; }

		[Outlet]
		UIKit.UIView SyncingView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LogoutButton != null) {
				LogoutButton.Dispose ();
				LogoutButton = null;
			}

			if (SyncedIcon != null) {
				SyncedIcon.Dispose ();
				SyncedIcon = null;
			}

			if (SyncedLabel != null) {
				SyncedLabel.Dispose ();
				SyncedLabel = null;
			}

			if (SyncedView != null) {
				SyncedView.Dispose ();
				SyncedView = null;
			}

			if (SyncingIndicator != null) {
				SyncingIndicator.Dispose ();
				SyncingIndicator = null;
			}

			if (SyncingLabel != null) {
				SyncingLabel.Dispose ();
				SyncingLabel = null;
			}

			if (SyncingView != null) {
				SyncingView.Dispose ();
				SyncingView = null;
			}

			if (LoggingOutView != null) {
				LoggingOutView.Dispose ();
				LoggingOutView = null;
			}

			if (LoggingOutIndicator != null) {
				LoggingOutIndicator.Dispose ();
				LoggingOutIndicator = null;
			}

			if (LoggingOutLabel != null) {
				LoggingOutLabel.Dispose ();
				LoggingOutLabel = null;
			}
		}
	}
}
