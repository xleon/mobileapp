using System;
using Foundation;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Daneel.Extensions;
using Toggl.Core.UI.ViewModels.Reports;
using UIKit;
using System.Reactive.Disposables;
using Toggl.Daneel.Extensions.Reactive;
using System.Reactive.Linq;
using Toggl.Shared.Extensions;
using System.Linq;
using Toggl.Daneel.Cells;
using Toggl.Core;
using Toggl.Core.Extensions;
using ObjCRuntime;
using Toggl.Core.UI.Extensions;
using Color = Toggl.Core.UI.Helper.Color;

namespace Toggl.Daneel.Views.Reports
{
    [Register(nameof(ReportsOverviewCardView))]
    public sealed partial class ReportsOverviewCardView : BaseReportsCardView<ReportsViewModel>
    {
        private const int fontSize = 24;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private static readonly UIColor normalColor = Color.Reports.PercentageActivated.ToNativeColor();
        private static readonly UIColor disabledColor = Color.Reports.Disabled.ToNativeColor();

        private readonly UIStringAttributes normalAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = normalColor
        };

        private readonly UIStringAttributes disabledAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = disabledColor
        };

        public ReportsOverviewCardView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static ReportsOverviewCardView CreateFromNib()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(ReportsOverviewCardView), null, null);
            return Runtime.GetNSObject<ReportsOverviewCardView>(arr.ValueAt(0));
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TotalTitleLabel.Text = Resources.Total.ToUpper();
            BillableTitleLabel.Text = Resources.Billable.ToUpper();

            var templateImage = TotalDurationGraph.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            TotalDurationGraph.Image = templateImage;

            prepareViews();
        }

        protected override void UpdateViewBinding()
        {
            //Text
            Item.BillablePercentageObservable
                .Select(billableFormattedString)
                .Subscribe(BillablePercentageLabel.Rx().AttributedText())
                .DisposedBy(disposeBag);

            Item.TotalTimeObservable
                .CombineLatest(Item.DurationFormatObservable,
                    (totalTime, durationFormat) => totalTime.ToFormattedString(durationFormat))
                .Subscribe(TotalDurationLabel.Rx().Text())
                .DisposedBy(disposeBag);

            //Loading chart
            Item.IsLoadingObservable
                .Subscribe(LoadingOverviewView.Rx().IsVisibleWithFade())
                .DisposedBy(disposeBag);

            //Pretty stuff
            Item.BillablePercentageObservable
                .Subscribe(percentage => BillablePercentageView.Percentage = percentage)
                .DisposedBy(disposeBag);

            var totalDurationColorObservable = Item.TotalTimeIsZeroObservable
                .Select(isZero => isZero
                    ? Core.UI.Helper.Color.Reports.Disabled.ToNativeColor()
                    : Core.UI.Helper.Color.Reports.TotalTimeActivated.ToNativeColor());

            totalDurationColorObservable
                .Subscribe(TotalDurationGraph.Rx().TintColor())
                .DisposedBy(disposeBag);

            totalDurationColorObservable
                .Subscribe(TotalDurationLabel.Rx().TextColor())
                .DisposedBy(disposeBag);

            NSAttributedString billableFormattedString(float? value)
            {
                var isDisabled = value == null;
                var actualValue = isDisabled ? 0 : value.Value;

                var percentage = $"{actualValue.ToString("0.00")}%";

                var attributes = isDisabled ? disabledAttributes : normalAttributes;
                return new NSAttributedString(percentage, attributes);
            }

            LayoutIfNeeded();
        }

        private void prepareViews()
        {
            PrepareCard(OverviewCardView);

            TotalTitleLabel.SetKerning(-0.2);
            TotalDurationLabel.SetKerning(-0.2);
            BillableTitleLabel.SetKerning(-0.2);
            BillablePercentageLabel.SetKerning(-0.2);
        }
    }
}
