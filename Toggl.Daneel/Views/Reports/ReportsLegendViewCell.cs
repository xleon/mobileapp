using System;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views.Reports
{
    public partial class ReportsLegendViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ReportsLegendViewCell));
        public static readonly UINib Nib;

        static ReportsLegendViewCell()
        {
            Nib = UINib.FromName(nameof(ReportsLegendViewCell), NSBundle.MainBundle);
        }

        protected ReportsLegendViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
