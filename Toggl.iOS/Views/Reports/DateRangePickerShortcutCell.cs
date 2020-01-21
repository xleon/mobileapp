using Foundation;
using System;
using Toggl.Core.UI.Parameters;
using Toggl.iOS.Extensions;
using UIKit;
using static Toggl.Core.UI.ViewModels.DateRangePicker.DateRangePickerViewModel;

namespace Toggl.iOS.Views.Reports
{
    public sealed partial class DateRangePickerShortcutCell : ReactiveCollectionViewCell<Shortcut>
    {
        public static readonly NSString Key = new NSString(nameof(DateRangePickerShortcutCell));
        public static readonly UINib Nib;

        static DateRangePickerShortcutCell()
        {
            Nib = UINib.FromName(nameof(DateRangePickerShortcutCell), NSBundle.MainBundle);
        }

        public DateRangePickerShortcutCell(IntPtr handle)
            : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TitleLabel.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Medium);
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Text;

            ContentView.BackgroundColor = Item.IsSelected
                ? ColorAssets.CustomGray
                : ColorAssets.CustomGray5;

            TitleLabel.TextColor = Item.IsSelected
                ? ColorAssets.InverseText
                : ColorAssets.Text2;
        }
    }
}
