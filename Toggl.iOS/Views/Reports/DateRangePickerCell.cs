using Foundation;
using System;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    public struct DatePickerCellData
    {
        public DateTime Date { get; }
        public bool IsToday { get; }
        public bool IsSelected { get; }
        public bool IsInCurrentMonth { get; }
        public bool IsStartOfSelectedPeriod { get; }
        public bool IsEndOfSelectedPeriod { get; }
        public bool IsPartial { get; }

        public DatePickerCellData(
            DateTime date,
            bool isToday,
            bool isSelected,
            bool isInCurrentMonth,
            bool isStartOfSelectedPeriod,
            bool isEndOfSelectedPeriod,
            bool isPartial)
        {
            Date = date;
            IsToday = isToday;
            IsSelected = isSelected;
            IsInCurrentMonth = isInCurrentMonth;
            IsStartOfSelectedPeriod = isStartOfSelectedPeriod;
            IsEndOfSelectedPeriod = isEndOfSelectedPeriod;
            IsPartial = isPartial;
        }
    }

    public sealed partial class DateRangePickerCell :
        ReactiveCollectionViewCell<DatePickerCellData>
    {
        private const int backgroundCornerRadius = 16;
        private const int todayCornerRadius = 14;

        public static readonly NSString Key = new NSString(nameof(DateRangePickerCell));
        public static readonly UINib Nib;

        static DateRangePickerCell()
        {
            Nib = UINib.FromName(nameof(DateRangePickerCell), NSBundle.MainBundle);
        }

        public DateRangePickerCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            prepareViews();
        }

        private void prepareViews()
        {
            //Background view
            BackgroundView.CornerRadius = backgroundCornerRadius;
            BackgroundView.RoundLeft = true;
            BackgroundView.RoundRight = true;
            BackgroundView.Layer.BorderWidth = 2;

            //Today background indicator
            TodayBackgroundView.CornerRadius = todayCornerRadius;
            TodayBackgroundView.RoundLeft = true;
            TodayBackgroundView.RoundRight = true;
            TodayBackgroundView.BackgroundColor = ColorAssets.CustomGray2;
        }

        private readonly UIColor otherMonthColor = ColorAssets.Text4;
        private readonly UIColor thisMonthColor = ColorAssets.Text2;
        private readonly UIColor selectedColor = ColorAssets.InverseText;
        private readonly UIColor todayColor = ColorAssets.InverseText;

        protected override void UpdateView()
        {
            Text.Text = Item.Date.Day.ToString();
            Text.TextColor = selectTextColor();

            BackgroundView.BackgroundColor = Item.IsSelected && (!Item.IsPartial || Item.IsToday)
                ? ColorAssets.CustomGray
                : Colors.Common.Transparent.ToNativeColor();

            BackgroundView.Layer.BorderColor = Item.IsSelected && Item.IsPartial
                ? ColorAssets.CustomGray.CGColor
                : Colors.Common.Transparent.ToNativeColor().CGColor;

            LeftBackgroundView.BackgroundColor = Item.IsSelected && !Item.IsStartOfSelectedPeriod
                ? ColorAssets.CustomGray
                : Colors.Common.Transparent.ToNativeColor();

            RightBackgroundView.BackgroundColor = Item.IsSelected && !Item.IsEndOfSelectedPeriod
                ? ColorAssets.CustomGray
                : Colors.Common.Transparent.ToNativeColor();

            TodayBackgroundView.Hidden = !Item.IsToday;
        }

        private UIColor selectTextColor()
        {
            if (Item.IsToday)
            {
                return Item.IsSelected
                    ? selectedColor
                    : todayColor;
            }

            if (Item.IsSelected && !Item.IsPartial)
            {
                return selectedColor;
            }

            if (Item.IsInCurrentMonth)
            {
                return thisMonthColor;
            }

            return otherMonthColor;
        }
    }
}
