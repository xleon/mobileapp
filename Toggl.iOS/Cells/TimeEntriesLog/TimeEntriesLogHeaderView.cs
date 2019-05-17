using System;
using System.Reactive.Disposables;
using Foundation;
using Toggl.iOS.Extensions;
using UIKit;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.iOS.Cells;

namespace Toggl.iOS.Views
{
    public partial class TimeEntriesLogHeaderView : BaseTableHeaderFooterView<DaySummaryViewModel>
    {
        private const double maxWidth = 834;
        public static readonly string Identifier = "timeEntryLogHeaderCell";

        public static readonly NSString Key = new NSString(nameof(TimeEntriesLogHeaderView));
        public static readonly UINib Nib;

        static TimeEntriesLogHeaderView()
        {
            Nib = UINib.FromName(nameof(TimeEntriesLogHeaderView), NSBundle.MainBundle);
        }

        protected TimeEntriesLogHeaderView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ContentView.BackgroundColor = UIColor.White;
            DurationLabel.Font = DurationLabel.Font.GetMonospacedDigitFont();
        }

        protected override void UpdateView()
        {
            DateLabel.Text = Item.Title;
            DurationLabel.Text = Item.TotalTrackedTime;
        }

        public override void LayoutSubviews()
        {
            if (Superview != null)
                ContentWidthConstraint.Constant = (nfloat)Math.Min(Superview.Bounds.Width, maxWidth);
            base.LayoutSubviews();
        }
    }
}
