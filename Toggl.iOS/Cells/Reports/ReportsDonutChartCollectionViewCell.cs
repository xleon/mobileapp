using System;

using Foundation;
using UIKit;

namespace Toggl.iOS.Cells.Reports
{
    public partial class ReportsDonutChartCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ReportsDonutChartCollectionViewCell");
        public static readonly UINib Nib;
        public static readonly int Height = 307;

        static ReportsDonutChartCollectionViewCell()
        {
            Nib = UINib.FromName("ReportsDonutChartCollectionViewCell", NSBundle.MainBundle);
        }

        protected ReportsDonutChartCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}

