using System;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntriesLogViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(TimeEntriesLogViewCell));
        public static readonly UINib Nib;

        static TimeEntriesLogViewCell()
        {
            Nib = UINib.FromName(nameof(TimeEntriesLogViewCell), NSBundle.MainBundle);
        }

        protected TimeEntriesLogViewCell(IntPtr handle) 
            : base(handle)
        {
        }
    }
}
