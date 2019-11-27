using System;

using Foundation;
using UIKit;

namespace Toggl.iOS.Cells.Reports
{
    public partial class ReportsNoDataCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ReportsNoDataCollectionViewCell");
        public static readonly UINib Nib;

        static ReportsNoDataCollectionViewCell()
        {
            Nib = UINib.FromName("ReportsNoDataCollectionViewCell", NSBundle.MainBundle);
        }

        protected ReportsNoDataCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}

