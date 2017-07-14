using System;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class SuggestionsViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(SuggestionsViewCell));
        public static readonly UINib Nib;

        static SuggestionsViewCell()
        {
            Nib = UINib.FromName(nameof(SuggestionsViewCell), NSBundle.MainBundle);
        }

        protected SuggestionsViewCell(IntPtr handle) 
            : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
