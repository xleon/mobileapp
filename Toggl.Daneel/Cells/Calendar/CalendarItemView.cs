using System;
using Foundation;
using MvvmCross.Plugin.Color.Platforms.Ios;
using MvvmCross.UI;
using Toggl.Daneel.Views;
using Toggl.Foundation.Calendar;
using UIKit;

namespace Toggl.Daneel.Cells.Calendar
{
    public sealed partial class CalendarItemView : ReactiveCollectionViewCell<CalendarItem>
    {
        public static readonly NSString Key = new NSString(nameof(CalendarItemView));
        public static readonly UINib Nib;

        static CalendarItemView()
        {
            Nib = UINib.FromName(nameof(CalendarItemView), NSBundle.MainBundle);
        }

        protected CalendarItemView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            CalendarIconImageView.Image = UIImage
                .FromBundle("icCalendarSmall")
                .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }

        protected override void UpdateView()
        {
            var color = MvxColor.ParseHexString(Item.Color).ToNativeColor();
            DescriptionLabel.Text = Item.Description;
            DescriptionLabel.TextColor = textColor(Item.Source, color);
            ColorView.BackgroundColor = backgroundColor(Item.Source, color);
            setCalendarIconVisibility(Item.Source, color);
        }

        private UIColor backgroundColor(CalendarItemSource source, UIColor color)
        {
            switch (source)
            {
                case CalendarItemSource.Calendar:
                    return color.ColorWithAlpha((nfloat)0.24);
                case CalendarItemSource.TimeEntry:
                    return color;
                default:
                    throw new ArgumentException("Unexpected calendar item source");
            }
        }

        private UIColor textColor(CalendarItemSource source, UIColor color)
        {
            switch (source)
            {
                case CalendarItemSource.Calendar:
                    return color;
                case CalendarItemSource.TimeEntry:
                    return UIColor.White;
                default:
                    throw new ArgumentException("Unexpected calendar item source");
            }
        }

        private void setCalendarIconVisibility(CalendarItemSource source, UIColor color)
        {
            switch (Item.Source)
            {
                case CalendarItemSource.Calendar:
                    CalendarIconImageView.TintColor = color;
                    CalendarIconImageView.Hidden = false;
                    CalendarIconLeadingConstraint.Active = true;
                    CalendarIconTrailingConstraint.Active = true;
                    break;
                case CalendarItemSource.TimeEntry:
                    CalendarIconImageView.Hidden = true;
                    CalendarIconLeadingConstraint.Active = false;
                    CalendarIconTrailingConstraint.Active = false;
                    break;
                default:
                    throw new ArgumentException("Unexpected calendar item source");
            }
        }
    }
}
