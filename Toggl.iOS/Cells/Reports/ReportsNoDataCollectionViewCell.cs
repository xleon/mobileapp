using System;
using Toggl.Shared;
using Foundation;
using UIKit;
using Toggl.iOS.Extensions;

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

        override public void AwakeFromNib()
        {
            base.AwakeFromNib();
            
            ErrorTitleLabel.Text = Resources.ReportsEmptyStateTitle;
            ErrorMessageLabel.Text = Resources.ReportsEmptyStateDescription;

            ErrorTitleLabel.SetKerning(-0.2);
            ErrorMessageLabel.SetKerning(-0.2);
        }
    }
}

