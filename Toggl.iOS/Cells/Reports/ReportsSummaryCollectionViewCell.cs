using System;

using Foundation;
using UIKit;

namespace Toggl.iOS.Cells.Reports
{
    public partial class ReportsSummaryCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ReportsSummaryCollectionViewCell");
        public static readonly UINib Nib;

        public static readonly int Height = 124;

        static ReportsSummaryCollectionViewCell()
        {
            Nib = UINib.FromName("ReportsSummaryCollectionViewCell", NSBundle.MainBundle);
        }

        protected ReportsSummaryCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}

