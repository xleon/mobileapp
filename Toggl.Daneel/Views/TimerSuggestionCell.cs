using System;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class TimerSuggestionCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(TimerSuggestionCell));
        public static readonly UINib Nib;

        static TimerSuggestionCell()
        {
            Nib = UINib.FromName(nameof(TimerSuggestionCell), NSBundle.MainBundle);
        }

        protected TimerSuggestionCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
