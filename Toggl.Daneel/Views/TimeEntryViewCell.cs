using System;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntryViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(TimeEntryViewCell));
        public static readonly UINib Nib;

        static TimeEntryViewCell()
        {
            Nib = UINib.FromName(nameof(TimeEntryViewCell), NSBundle.MainBundle);
        }

        protected TimeEntryViewCell(IntPtr handle) 
            : base(handle)
        {
        }
    }
}
