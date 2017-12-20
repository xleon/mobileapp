// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.Views.Reports
{
    [Register ("ReportsHeaderView")]
    partial class ReportsHeaderView
    {
        [Outlet]
        UIKit.UILabel BillablePercentageLabel { get; set; }

        [Outlet]
        Toggl.Daneel.Views.Reports.PercentageView BillablePercentageView { get; set; }

        [Outlet]
        UIKit.UIView EmptyStateView { get; set; }

        [Outlet]
        Toggl.Daneel.Views.Reports.PieChartView PieChartView { get; set; }

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

            if (PieChartView != null) {
                PieChartView.Dispose ();
                PieChartView = null;
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