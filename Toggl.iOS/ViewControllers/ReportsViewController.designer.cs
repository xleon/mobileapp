// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.ViewControllers
{
	[Register ("ReportsViewController")]
	partial class ReportsViewController
	{
		[Outlet]
		UIKit.UICollectionView CollectionView { get; set; }

		[Outlet]
		UIKit.UIView WorkspaceButton { get; set; }

		[Outlet]
		Toggl.iOS.Views.FadeView WorkspaceFadeView { get; set; }

		[Outlet]
		UIKit.UILabel WorkspaceLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (WorkspaceButton != null) {
				WorkspaceButton.Dispose ();
				WorkspaceButton = null;
			}

			if (WorkspaceFadeView != null) {
				WorkspaceFadeView.Dispose ();
				WorkspaceFadeView = null;
			}

			if (WorkspaceLabel != null) {
				WorkspaceLabel.Dispose ();
				WorkspaceLabel = null;
			}

			if (CollectionView != null) {
				CollectionView.Dispose ();
				CollectionView = null;
			}
		}
	}
}
