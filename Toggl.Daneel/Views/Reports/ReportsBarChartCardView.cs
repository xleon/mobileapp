using System;
using Foundation;
using Toggl.Daneel.Extensions;
using Toggl.Core.UI.ViewModels.Reports;
using UIKit;
using System.Reactive.Disposables;
using Toggl.Daneel.Extensions.Reactive;
using System.Reactive.Linq;
using Toggl.Shared.Extensions;
using System.Linq;
using Toggl.Shared;
using System.Collections.Generic;
using System.Globalization;
using Toggl.Core.Conversions;
using System.Reactive.Subjects;
using System.Reactive;
using Toggl.Daneel.Cells;
using Toggl.Core;
using ObjCRuntime;

namespace Toggl.Daneel.Views.Reports
{
    [Register(nameof(ReportsBarChartCardView))]
    public partial class ReportsBarChartCardView : BaseReportsCardView<ReportsViewModel>
    {
        private const float barChartSpacingProportion = 0.3f;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        private readonly ISubject<Unit> updateLayout = new BehaviorSubject<Unit>(Unit.Default);

        public ReportsBarChartCardView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static ReportsBarChartCardView CreateFromNib()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(ReportsBarChartCardView), null, null);
            return Runtime.GetNSObject<ReportsBarChartCardView>(arr.ValueAt(0));
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ClockedHoursTitleLabel.Text = Resources.ClockedHours.ToUpper();
            BillableLegendLabel.Text = Resources.Billable.ToUpper();
            NonBillableLegendLabel.Text = Resources.NonBillable.ToUpper();

            prepareViews();
        }

        protected override void UpdateViewBinding()
        {
            // Bar chart
            Item.WorkspaceHasBillableFeatureEnabled
                .Subscribe(ColorsLegendContainerView.Rx().IsVisible())
                .DisposedBy(disposeBag);

            Item.StartDate
                .CombineLatest(
                    Item.BarChartViewModel.DateFormat,
                    (startDate, format) => startDate.ToString(format.Short, CultureInfo.InvariantCulture))
                .Subscribe(StartDateLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.EndDate
                .CombineLatest(
                    Item.BarChartViewModel.DateFormat,
                    (endDate, format) => endDate.ToString(format.Short, CultureInfo.InvariantCulture))
                .Subscribe(EndDateLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.MaximumHoursPerBar
                .Select(hours => $"{hours} h")
                .Subscribe(MaximumHoursLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.MaximumHoursPerBar
                .Select(hours => $"{hours / 2} h")
                .Subscribe(HalfHoursLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.HorizontalLegend
                .Where(legend => legend == null)
                .Subscribe((DateTimeOffset[] _) =>
                {
                    HorizontalLegendStackView.Subviews.ForEach(subview => subview.RemoveFromSuperview());
                    StartDateLabel.Hidden = false;
                    EndDateLabel.Hidden = false;
                })
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.HorizontalLegend
                .Where(legend => legend != null)
                .CombineLatest(Item.BarChartViewModel.DateFormat, createHorizontalLegendLabels)
                .Do(_ =>
                {
                    StartDateLabel.Hidden = true;
                    EndDateLabel.Hidden = true;
                })
                .Subscribe(HorizontalLegendStackView.Rx().ArrangedViews())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.Bars
                .Select(createBarViews)
                .Subscribe(BarsStackView.Rx().ArrangedViews())
                .DisposedBy(disposeBag);

            var spacingObservable = Item.BarChartViewModel.Bars
                .CombineLatest(updateLayout, (bars, _) => bars)
                .Select(bars => BarsStackView.Frame.Width / bars.Length * barChartSpacingProportion);

            spacingObservable
                .Subscribe(BarsStackView.Rx().Spacing())
                .DisposedBy(disposeBag);

            spacingObservable
                .Subscribe(HorizontalLegendStackView.Rx().Spacing())
                .DisposedBy(disposeBag);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Hidden)
            {
                return;
            }

            updateLayout.OnNext(Unit.Default);
        }

        private void prepareViews()
        {
            PrepareCard(BarChartCardView);

            ClockedHoursTitleLabel.SetKerning(-0.2);
            BillableLegendLabel.SetKerning(-0.2);
            NonBillableLegendLabel.SetKerning(-0.2);
        }

        private IEnumerable<UIView> createBarViews(IEnumerable<BarViewModel> bars)
            => bars.Select<BarViewModel, UIView>(bar =>
            {
                if (bar.NonBillablePercent == 0 && bar.BillablePercent == 0)
                {
                    return new EmptyBarView();
                }

                return new BarView(bar);
            });

        private IEnumerable<UILabel> createHorizontalLegendLabels(IEnumerable<DateTimeOffset> dates, DateFormat format)
            => dates.Select(date =>
                new BarLegendLabel(
                    DateTimeOffsetConversion.ToDayOfWeekInitial(date),
                    date.ToString(format.Short, CultureInfo.InvariantCulture)));
    }
}
