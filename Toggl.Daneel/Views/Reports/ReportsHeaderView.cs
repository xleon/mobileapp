using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Toggl.Daneel.Cells;
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
using Toggl.Core.Extensions;
using Color = Toggl.Core.UI.Helper.Color;

namespace Toggl.Daneel.Views.Reports
{
    public partial class ReportsHeaderView : BaseTableHeaderFooterView<ReportsViewModel>
    {
        public static readonly string Identifier = nameof(ReportsHeaderView);
        public static readonly NSString Key = new NSString(nameof(ReportsHeaderView));
        public static readonly UINib Nib;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private ReportsOverviewCardView overview = ReportsOverviewCardView.CreateFromNib();
        private ReportsBarChartCardView barChart = ReportsBarChartCardView.CreateFromNib();

        private CAShapeLayer borderLayer = new CAShapeLayer();
        private CAShapeLayer mask = new CAShapeLayer();

        static ReportsHeaderView()
        {
            Nib = UINib.FromName(nameof(ReportsHeaderView), NSBundle.MainBundle);
        }

        public ReportsHeaderView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            OverviewContainerView.AddSubview(overview);
            BarChartContainerView.AddSubview(barChart);
            overview.Frame = OverviewContainerView.Bounds;
            barChart.Frame = BarChartContainerView.Bounds;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (BarChartContainerView != null)
            {
                BarChartContainerView.Hidden = TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular;
            }

            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
            {
                var cornerRadius = 8;
                var cornersToRound = PieChartView.Segments.Count() > 0
                    ? UIRectCorner.TopLeft | UIRectCorner.TopRight
                    : UIRectCorner.TopLeft | UIRectCorner.TopRight | UIRectCorner.BottomLeft | UIRectCorner.BottomRight;

                mask.Path = UIBezierPath.FromRoundedRect(Bounds, cornersToRound, new CGSize(cornerRadius, cornerRadius)).CGPath;
                Layer.Mask = mask;

                borderLayer.FillColor = UIColor.Clear.CGColor;
                borderLayer.LineWidth = 1;
                borderLayer.StrokeColor = UIColor.GroupTableViewBackgroundColor.CGColor;
                borderLayer.Path = UIBezierPath.FromRoundedRect(new CGRect(0.5, 0.5, Bounds.Width - 1, Bounds.Height - 1), cornersToRound, new CGSize(cornerRadius + 1, cornerRadius + 1)).CGPath;
                Layer.AddSublayer(borderLayer);
            }
            else
            {
                Layer.Mask = null;
                borderLayer.RemoveFromSuperLayer();
            }
        }

        protected override void UpdateView()
        {
            overview.Item = Item;
            barChart.Item = Item;

            //Loading chart
            Item.IsLoadingObservable
                .Subscribe(LoadingPieChartView.Rx().IsVisibleWithFade())
                .DisposedBy(disposeBag);

            //Pretty stuff
            Item.GroupedSegmentsObservable
                .Subscribe(groupedSegments => PieChartView.Segments = groupedSegments)
                .DisposedBy(disposeBag);

            Item.IsLoadingObservable
                .Select(CommonFunctions.Invert)
                .Subscribe(BarChartContainerView.Rx().IsVisible())
                .DisposedBy(disposeBag);

            //Visibility
            Item.ShowEmptyStateObservable
                .Subscribe(EmptyStateView.Rx().IsVisible())
                .DisposedBy(disposeBag);
        }
    }
}
