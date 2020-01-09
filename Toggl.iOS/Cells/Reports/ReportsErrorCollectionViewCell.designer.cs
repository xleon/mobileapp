// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Cells.Reports
{
	[Register ("ReportsErrorCollectionViewCell")]
	partial class ReportsErrorCollectionViewCell
	{
		[Outlet]
		UIKit.UILabel ErrorMessageLabel { get; set; }

		[Outlet]
		UIKit.UILabel ErrorTitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ErrorMessageLabel != null) {
				ErrorMessageLabel.Dispose ();
				ErrorMessageLabel = null;
			}

			if (ErrorTitleLabel != null) {
				ErrorTitleLabel.Dispose ();
				ErrorTitleLabel = null;
			}
		}
	}
}
