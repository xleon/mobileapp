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
	[Register ("ReportsDonutChartCollectionViewCell")]
	partial class ReportsDonutChartCollectionViewCell
	{
		[Outlet]
		Toggl.iOS.Views.Reports.DonutChartView DonutChartView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DonutChartView != null) {
				DonutChartView.Dispose ();
				DonutChartView = null;
			}
		}
	}
}
