using System;

using Foundation;
using UIKit;

namespace Toggl.iOS.Cells.Reports
{
    public partial class ReportsDonutChartLegendCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ReportsDonutChartLegendCollectionViewCell");
        public static readonly UINib Nib;
        public static readonly int Height = 56;

        static ReportsDonutChartLegendCollectionViewCell()
        {
            Nib = UINib.FromName("ReportsDonutChartLegendCollectionViewCell", NSBundle.MainBundle);
        }

        protected ReportsDonutChartLegendCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}

