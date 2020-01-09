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
	[Register ("ReportsSummaryCollectionViewCell")]
	partial class ReportsSummaryCollectionViewCell
	{
		[Outlet]
		UIKit.UILabel BillablePercentageLabel { get; set; }

		[Outlet]
		UIKit.UILabel BillableTitleLabel { get; set; }

		[Outlet]
		UIKit.UIView ContainerView { get; set; }

		[Outlet]
		UIKit.UIStackView LoadingView { get; set; }

		[Outlet]
		UIKit.UILabel TotalTimeLabel { get; set; }

		[Outlet]
		UIKit.UILabel TotalTimeTitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BillablePercentageLabel != null) {
				BillablePercentageLabel.Dispose ();
				BillablePercentageLabel = null;
			}

			if (BillableTitleLabel != null) {
				BillableTitleLabel.Dispose ();
				BillableTitleLabel = null;
			}

			if (ContainerView != null) {
				ContainerView.Dispose ();
				ContainerView = null;
			}

			if (LoadingView != null) {
				LoadingView.Dispose ();
				LoadingView = null;
			}

			if (TotalTimeLabel != null) {
				TotalTimeLabel.Dispose ();
				TotalTimeLabel = null;
			}

			if (TotalTimeTitleLabel != null) {
				TotalTimeTitleLabel.Dispose ();
				TotalTimeTitleLabel = null;
			}
		}
	}
}
