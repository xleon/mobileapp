using System;
using System.Linq;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntryViewCell : MvxTableViewCell
    {
        private static readonly NSString Key = new NSString(nameof(TimeEntryViewCell));
        private static readonly UINib Nib;

        public static TimeEntryViewCell FromNib()
            => Nib.Instantiate(null, null).First() as TimeEntryViewCell;

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
