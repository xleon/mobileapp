using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
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
        private static readonly Dictionary<CalendarIconKind, UIImage> images;

        private CAShapeLayer topDragIndicatorBorderLayer;
        private CAShapeLayer bottomDragIndicatorBorderLayer;

        public static readonly NSString Key = new NSString(nameof(CalendarItemView));
        public static readonly UINib Nib;

        public CGRect TopDragTouchArea => TopDragIndicator.Frame.Inset(-20, -20);
        public CGRect BottomDragTouchArea => BottomDragIndicator.Frame.Inset(-20, -20);

        private bool isEditing;
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                isEditing = value;
                updateDragIndicators(itemColor());
            }
        }

        static CalendarItemView()
        {
            Nib = UINib.FromName(nameof(CalendarItemView), NSBundle.MainBundle);

            images = new Dictionary<CalendarIconKind, UIImage>
            {
                { CalendarIconKind.Unsynced, templateImage("icUnsynced") },
                { CalendarIconKind.Event, templateImage("icCalendarSmall") },
                { CalendarIconKind.Unsyncable, templateImage("icErrorSmall") }
            };

            UIImage templateImage(string iconName)
                => UIImage.FromBundle(iconName)
                      .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }

        public CalendarItemView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            CalendarIconWidthConstrarint.Constant = 8;
            CalendarIconHeightConstrarint.Constant = 8;

            topDragIndicatorBorderLayer = new CAShapeLayer();
            configureDragIndicatorBorderLayer(TopDragIndicator, topDragIndicatorBorderLayer);
            bottomDragIndicatorBorderLayer = new CAShapeLayer();
            configureDragIndicatorBorderLayer(BottomDragIndicator, bottomDragIndicatorBorderLayer);

            void configureDragIndicatorBorderLayer(UIView dragIndicator, CAShapeLayer borderLayer)
            {
                var rect = dragIndicator.Bounds.Inset(1, 1);
                borderLayer.Path = UIBezierPath.FromOval(rect).CGPath;
                borderLayer.BorderWidth = 2;
                borderLayer.FillColor = UIColor.Clear.CGColor;
                dragIndicator.Layer.AddSublayer(borderLayer);
            }
        }

        protected override void UpdateView()
        {
            var color = itemColor();
            DescriptionLabel.Text = Item.Description;
            DescriptionLabel.TextColor = textColor(color);
            ColorView.BackgroundColor = backgroundColor(Item.Source, color);
            updateIcon(color);
            updateSizes();
            updateDragIndicators(color);
        }

        private UIColor itemColor()
            => MvxColor.ParseHexString(Item.Color).ToNativeColor();

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

        private UIColor textColor(UIColor color)
        {
            switch (Item.Source)
            {
                case CalendarItemSource.Calendar:
                    return color;
                case CalendarItemSource.TimeEntry:
                    return UIColor.White;
                default:
                    throw new ArgumentException("Unexpected calendar item source");
            }
        }

        private void updateIcon(UIColor color)
        {
            if (Item.IconKind == CalendarIconKind.None)
            {
                CalendarIconImageView.Hidden = true;
                CalendarIconLeadingConstraint.Active = false;
                CalendarIconTrailingConstraint.Active = false;
                return;
            }

            CalendarIconImageView.Hidden = false;
            CalendarIconLeadingConstraint.Active = true;
            CalendarIconTrailingConstraint.Active = true;
            CalendarIconImageView.TintColor = textColor(color);
            CalendarIconImageView.Image = images[Item.IconKind];
        }

        private void updateDragIndicators(UIColor color)
        {
            TopDragIndicator.Hidden = !IsEditing;
            BottomDragIndicator.Hidden = !IsEditing;
            topDragIndicatorBorderLayer.StrokeColor = color.CGColor;
            bottomDragIndicatorBorderLayer.StrokeColor = color.CGColor;
        }

        private void updateSizes()
        {
            if (Item.Duration < TimeSpan.FromMinutes(30))
            {
                DescriptionLabelTopConstraint.Constant = 0;
                DescriptionLabelBottomConstraint.Constant = 0;
                CalendarIconWidthConstrarint.Active = true;
                CalendarIconHeightConstrarint.Active = true;
                CalendarIconBaselineConstraint.Active = false;
            }
            else
            {
                DescriptionLabelTopConstraint.Constant = 6;
                DescriptionLabelBottomConstraint.Constant = 6;
                CalendarIconWidthConstrarint.Active = false;
                CalendarIconHeightConstrarint.Active = false;
                CalendarIconBaselineConstraint.Active = true;
            }
        }
    }
}
