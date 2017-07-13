using System;
using System.Linq;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntryLogHeaderViewCell : UITableViewHeaderFooterView
    {
        private static readonly NSString Key = new NSString(nameof(TimeEntryLogHeaderViewCell));
        private static readonly UINib Nib;

        public static TimeEntryLogHeaderViewCell FromNib()
            => Nib.Instantiate(null, null).First() as TimeEntryLogHeaderViewCell;

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
