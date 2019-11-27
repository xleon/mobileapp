using System;

using Foundation;
using UIKit;

namespace Toggl.iOS.Cells.Reports
{
    public partial class ReportsErrorCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ReportsErrorCollectionViewCell");
        public static readonly UINib Nib;

        static ReportsErrorCollectionViewCell()
        {
            Nib = UINib.FromName("ReportsErrorCollectionViewCell", NSBundle.MainBundle);
        }

        protected ReportsErrorCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}

