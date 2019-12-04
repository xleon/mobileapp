using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CoreGraphics;
using Foundation;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Extensions;
using Toggl.iOS.Views.Reports;
using Toggl.Shared;
using UIKit;
using static Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement;

namespace Toggl.iOS.Cells.Reports
{
    public partial class ReportsBarChartCollectionViewCell : UICollectionViewCell
    {
        private const float barChartSpacingProportion = 0.3f;

        public static readonly NSString Key = new NSString("ReportsBarChartCollectionViewCell");
        public static readonly UINib Nib;
        public static readonly int Height = 270;

        static ReportsBarChartCollectionViewCell()
        {
            Nib = UINib.FromName("ReportsBarChartCollectionViewCell", NSBundle.MainBundle);
        }

        protected ReportsBarChartCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ClockedHoursTitleLabel.Text = Resources.ClockedHours.ToUpper();
            BillableLegendLabel.Text = Resources.Billable.ToUpper();
            NonBillableLegendLabel.Text = Resources.NonBillable.ToUpper();
            ColorsLegendContainerView.Hidden = false;

            ContentView.Layer.MasksToBounds = true;
            ContentView.Layer.CornerRadius = 8;
            Layer.MasksToBounds = false;
            Layer.CornerRadius = 8;
            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowRadius = 8;
            Layer.ShadowOffset = new CGSize(0, 2);
            Layer.ShadowOpacity = 0.1f;

            ClockedHoursTitleLabel.SetKerning(-0.2);
            BillableLegendLabel.SetKerning(-0.2);
            NonBillableLegendLabel.SetKerning(-0.2);
        }

        public void SetElement(ReportBarChartElement element)
        {
            if (element.Bars.IsEmpty)
            {
                setupEmptyElement();
                return;
            }

            MaximumHoursLabel.Hidden = false;
            HalfHoursLabel.Hidden = false;
            ZeroHoursLabel.Hidden = false;

            MaximumHoursLabel.Text = element.YLabels.TopLabel;
            HalfHoursLabel.Text = element.YLabels.MiddleLabel;
            ZeroHoursLabel.Text = element.YLabels.BottomLabel;

            foreach (var view in BarsStackView.ArrangedSubviews)
            {
                view.RemoveFromSuperview();
            }

            var barViews = createBarViews(element.Bars);
            foreach (var barView in barViews)
            {
                BarsStackView.AddArrangedSubview(barView);
            }
            BarsStackView.Spacing = (nfloat)getSpacingForNumberOfItems(element.Bars.Count);

            if (element.XLabels.Count == 2)
            {
                setupIndependentRangeXAxisLabels(element);
            }
            else if (element.XLabels.Count > 2)
            {
                setupBarBasedXAxisLabels(element);
            }
            else
            {
                HorizontalLegendStackView.Hidden = true;
                StartDateLabel.Hidden = true;
                EndDateLabel.Hidden = true;
            }
        }

        private void setupBarBasedXAxisLabels(ReportBarChartElement element)
        {
            foreach (var view in HorizontalLegendStackView.ArrangedSubviews)
            {
                view.RemoveFromSuperview();
            }

            var labels = createHorizontalLegendLabels(element.XLabels);
            foreach (var label in labels)
            {
                HorizontalLegendStackView.AddArrangedSubview(label);
            }
            HorizontalLegendStackView.Spacing = (nfloat)getSpacingForNumberOfItems(element.XLabels.Count);

            HorizontalLegendStackView.Hidden = false;
            StartDateLabel.Hidden = true;
            EndDateLabel.Hidden = true;
        }

        private void setupIndependentRangeXAxisLabels(ReportBarChartElement element)
        {
            StartDateLabel.Text = element.XLabels[0];
            EndDateLabel.Text = element.XLabels[1];

            HorizontalLegendStackView.Hidden = true;
            StartDateLabel.Hidden = false;
            EndDateLabel.Hidden = false;
        }

        private void setupEmptyElement()
        {
            MaximumHoursLabel.Hidden = true;
            HalfHoursLabel.Hidden = true;
            ZeroHoursLabel.Hidden = true;
            HorizontalLegendStackView.Hidden = true;
            StartDateLabel.Hidden = true;
            EndDateLabel.Hidden = true;

            foreach (var view in BarsStackView.ArrangedSubviews)
            {
                view.RemoveFromSuperview();
            }
            var placeholderBars = generatePlaceholderBars();
            foreach (var barView in placeholderBars)
            {
                BarsStackView.AddArrangedSubview(barView);
            }
            BarsStackView.Spacing = (nfloat)getSpacingForNumberOfItems(placeholderBars.Count);
        }

        private ImmutableList<UIView> createBarViews(IEnumerable<Bar> bars)
            => bars.Select(bar =>
            {
                return bar.TotalValue == 0 && bar.FilledValue == 0
                    ? (UIView)new EmptyBarView()
                    : (UIView)new BarView(bar.FilledValue, bar.TotalValue);
            })
            .ToImmutableList();

        private double getSpacingForNumberOfItems(int number)
            => BarsStackView.Frame.Width / number * barChartSpacingProportion;

        private IEnumerable<UILabel> createHorizontalLegendLabels(IEnumerable<string> dates)
            => dates.Select(date => new BarLegendLabel(date));

        private ImmutableList<BarView> generatePlaceholderBars()
        {
            var maxTotal = 73.8 + 72;
            return new (double Filled, double Regular)[] {
                (12.4, 6.9),
                (7.6, 13.5),
                (12.4, 6.9),
                (15.1, 18.9),
                (15.1, 20.1),
                (14.6, 24.5),
                (9.5, 28.2),
                (11.3, 23.9),
                (11.5, 32.9),
                (10.5, 22.8),
                (35.5, 24.5),
                (36.1, 24.2),
                (41.5, 31.2),
                (40, 33.3),
                (8.3, 43),
                (39.9, 35.6),
                (10, 50.9),
                (14.2, 46.7),
                (39.7, 50.2),
                (40.8, 44),
                (7.6, 43.9),
                (56.1, 55.7),
                (52.7, 53.2),
                (66.5, 50.7),
                (53.3, 54.6),
                (59, 64),
                (73, 61),
                (71.1, 70),
                (73.8, 72),
                (64.5, 79.8),
                (78.2, 67.3),
            }
            .Select(tuple => new BarView(
                tuple.Filled / maxTotal,
                (tuple.Filled + tuple.Regular) / maxTotal,
                ColorAssets.ReportsBarChartFilledPlaceholder,
                ColorAssets.ReportsBarChartTotalPlaceholder))
            .ToImmutableList();
        }
    }
}

