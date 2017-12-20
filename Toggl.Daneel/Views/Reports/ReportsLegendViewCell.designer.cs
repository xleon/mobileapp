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
    [Register ("ReportsLegendViewCell")]
    partial class ReportsLegendViewCell
    {
        [Outlet]
        UIKit.UIView CircleView { get; set; }


        [Outlet]
        UIKit.UILabel PercentageLabel { get; set; }


        [Outlet]
        UIKit.UILabel ProjectLabel { get; set; }


        [Outlet]
        UIKit.UILabel TotalTimeLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CircleView != null) {
                CircleView.Dispose ();
                CircleView = null;
            }

            if (PercentageLabel != null) {
                PercentageLabel.Dispose ();
                PercentageLabel = null;
            }

            if (ProjectLabel != null) {
                ProjectLabel.Dispose ();
                ProjectLabel = null;
            }

            if (TotalTimeLabel != null) {
                TotalTimeLabel.Dispose ();
                TotalTimeLabel = null;
            }
        }
    }
}