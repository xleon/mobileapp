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
	[Register ("ReportsHeaderView")]
	partial class ReportsHeaderView
	{
		[Outlet]
		UIKit.UIView BarChartContainerView { get; set; }

		[Outlet]
		UIKit.UIView EmptyStateView { get; set; }

		[Outlet]
		Toggl.Daneel.Views.Reports.LoadingPieChartView LoadingPieChartView { get; set; }

		[Outlet]
		UIKit.UIView OverviewContainerView { get; set; }

		[Outlet]
		Toggl.Daneel.Views.Reports.PieChartView PieChartView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BarChartContainerView != null) {
				BarChartContainerView.Dispose ();
				BarChartContainerView = null;
			}

			if (EmptyStateView != null) {
				EmptyStateView.Dispose ();
				EmptyStateView = null;
			}

			if (LoadingPieChartView != null) {
				LoadingPieChartView.Dispose ();
				LoadingPieChartView = null;
			}

			if (OverviewContainerView != null) {
				OverviewContainerView.Dispose ();
				OverviewContainerView = null;
			}

			if (PieChartView != null) {
				PieChartView.Dispose ();
				PieChartView = null;
			}
		}
	}
}
