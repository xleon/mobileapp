using System;
using Foundation;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Daneel.Converters;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.ViewModels.Reports;
using UIKit;
using System.Reactive.Disposables;
using Toggl.Daneel.Extensions.Reactive;
using System.Reactive.Linq;
using Toggl.Multivac.Extensions;
using System.Linq;
using Toggl.Daneel.Cells;
using Toggl.Foundation;
using Toggl.Foundation.Extensions;
using ObjCRuntime;
using Toggl.Foundation.MvvmCross.Extensions;

 namespace Toggl.Daneel.Views.Reports
{
    [Register(nameof(ReportsOverviewCardView))]
    public sealed partial class ReportsOverviewCardView : BaseReportsCardView<ReportsViewModel>
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

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
            var reportPercentageConverter = new ReportPercentageLabelValueConverter();
            Item.BillablePercentageObservable
                .Select(reportPercentageConverter.Convert)
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
                    ? Foundation.MvvmCross.Helper.Color.Reports.Disabled.ToNativeColor()
                    : Foundation.MvvmCross.Helper.Color.Reports.TotalTimeActivated.ToNativeColor());

            totalDurationColorObservable
                .Subscribe(TotalDurationGraph.Rx().TintColor())
                .DisposedBy(disposeBag);

            totalDurationColorObservable
                .Subscribe(TotalDurationLabel.Rx().TextColor())
                .DisposedBy(disposeBag);

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
