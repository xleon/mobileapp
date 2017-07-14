using System;
using System.Linq;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntriesLogHeaderViewCell : UITableViewHeaderFooterView
    {
        public static readonly NSString Key = new NSString(nameof(TimeEntriesLogHeaderViewCell));
        public static readonly UINib Nib;

        static TimeEntriesLogHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(TimeEntriesLogHeaderViewCell), NSBundle.MainBundle);
        }

        protected TimeEntriesLogHeaderViewCell(IntPtr handle)
            : base(handle)
        {
        }
    }
}
