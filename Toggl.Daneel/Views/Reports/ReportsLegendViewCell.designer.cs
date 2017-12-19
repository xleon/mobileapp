// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.Views.Reports
{
	[Register ("ReportsLegendViewCell")]
	partial class ReportsLegendViewCell
	{
		[Outlet]
		UIKit.UILabel PercentageLabel { get; set; }

		[Outlet]
		UIKit.UILabel ProjectClientLabel { get; set; }

		[Outlet]
		UIKit.UILabel TotalTimeLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProjectClientLabel != null) {
				ProjectClientLabel.Dispose ();
				ProjectClientLabel = null;
			}

			if (TotalTimeLabel != null) {
				TotalTimeLabel.Dispose ();
				TotalTimeLabel = null;
			}

			if (PercentageLabel != null) {
				PercentageLabel.Dispose ();
				PercentageLabel = null;
			}
		}
	}
}
