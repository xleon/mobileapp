using System;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntryLogHeaderViewCell : UITableViewHeaderFooterView
    {
        public static readonly NSString Key = new NSString(nameof(TimeEntryLogHeaderViewCell));
        public static readonly UINib Nib;

        static TimeEntryLogHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(TimeEntryLogHeaderViewCell), NSBundle.MainBundle);
        }

        protected TimeEntryLogHeaderViewCell(IntPtr handle)
            : base(handle)
        {
        }
    }
}
