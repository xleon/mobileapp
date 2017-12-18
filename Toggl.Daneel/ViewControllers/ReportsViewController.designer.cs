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
	[Register ("ReportsViewController")]
	partial class ReportsViewController
	{
		[Outlet]
		UIKit.UILabel BillablePercentageLabel { get; set; }

		[Outlet]
		Toggl.Daneel.Views.Reports.PercentageView BillablePercentageView { get; set; }

		[Outlet]
		UIKit.UIView EmptyStateView { get; set; }

		[Outlet]
		UIKit.UIImageView ImageView { get; set; }

		[Outlet]
		Toggl.Daneel.Views.Reports.PieChartView PieChartView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TopConstraint { get; set; }

		[Outlet]
		UIKit.UIImageView TotalDurationGraph { get; set; }

		[Outlet]
		UIKit.UILabel TotalDurationLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BillablePercentageLabel != null) {
				BillablePercentageLabel.Dispose ();
				BillablePercentageLabel = null;
			}

			if (BillablePercentageView != null) {
				BillablePercentageView.Dispose ();
				BillablePercentageView = null;
			}

			if (EmptyStateView != null) {
				EmptyStateView.Dispose ();
				EmptyStateView = null;
			}

			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (PieChartView != null) {
				PieChartView.Dispose ();
				PieChartView = null;
			}

			if (TopConstraint != null) {
				TopConstraint.Dispose ();
				TopConstraint = null;
			}

			if (TotalDurationGraph != null) {
				TotalDurationGraph.Dispose ();
				TotalDurationGraph = null;
			}

			if (TotalDurationLabel != null) {
				TotalDurationLabel.Dispose ();
				TotalDurationLabel = null;
			}
		}
	}
}
