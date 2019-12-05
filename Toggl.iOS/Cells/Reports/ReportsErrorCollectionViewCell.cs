using System;
using Toggl.iOS.Extensions;
using Foundation;
using UIKit;
using Toggl.Core.UI.ViewModels.Reports;

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

        override public void AwakeFromNib()
        {
            base.AwakeFromNib();

            ErrorTitleLabel.Text = "Ooooops!";

            ErrorTitleLabel.SetKerning(-0.2);
            ErrorMessageLabel.SetKerning(-0.2);
        }

        public void setElement(ReportErrorElement errorElement)
        {
            ErrorMessageLabel.Text = errorElement.Message;
        }
    }
}

