using System;

using Foundation;
using UIKit;

namespace Toggl.iOS.Cells.Reports
{
    public partial class ReportsBarChartCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ReportsBarChartCollectionViewCell");
        public static readonly UINib Nib;
        public static readonly int Height = 270;

        static ReportsBarChartCollectionViewCell()
        {
            Nib = UINib.FromName("ReportsBarChartCollectionViewCell", NSBundle.MainBundle);
        }

        protected ReportsBarChartCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}

