using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportBarChartElement : ReportElementBase
    {
        public ImmutableList<Bar> Bars { get; } = ImmutableList<Bar>.Empty;
        public ImmutableList<string> XLabels { get; } = ImmutableList<string>.Empty;
        public double MaxBarValue { get; } = 0;
        public YAxisLabels YLabels { get; } = YAxisLabels.Empty;

        public ReportBarChartElement(
            IEnumerable<Bar> bars,
            IEnumerable<string> xLabels,
            YAxisLabels yLabels,
            Func<Bar, Bar> scalingFunction = null)
            : base(false)
        {
            var barsList = bars.ToList();
            var upperLimit = upperValueLimit(barsList);

            MaxBarValue = upperLimit;
            scalingFunction ??= (bar => normalizeBar(bar, upperLimit));

            Bars = barsList
                .Select(scalingFunction)
                .ToImmutableList();

            XLabels = xLabels.ToImmutableList();
            YLabels = yLabels;
        }

        private ReportBarChartElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportBarChartElement LoadingState
            => new ReportBarChartElement(true);

        private double upperValueLimit(List<Bar> bars)
            => bars.Count > 0 ? bars.Max(bar => bar.TotalValue) : 0;

        private Bar normalizeBar(Bar bar, double maxValue)
            => bar.Scaled(maxValue);

        public override bool Equals(IReportElement other)
            => GetType() == other.GetType()
            && other is ReportBarChartElement barChartElement
            && barChartElement.IsLoading == IsLoading
            && barChartElement.Bars.SequenceEqual(Bars)
            && barChartElement.XLabels.SequenceEqual(XLabels)
            && barChartElement.YLabels == YLabels;

        public struct Bar
        {
            public double FilledValue { get; }
            public double TotalValue { get; }

            public Bar(double filledValue, double totalValue)
            {
                FilledValue = filledValue;
                TotalValue = totalValue;
            }

            public Bar Scaled(double maxValue) => new Bar(FilledValue / maxValue, TotalValue / maxValue);

            public override bool Equals(object obj)
                => obj is Bar bar
                   && bar.FilledValue == FilledValue
                   && bar.TotalValue == TotalValue;

            public override int GetHashCode()
                => HashCode.Combine(FilledValue, TotalValue);

            public static bool operator ==(Bar left, Bar right)
                => left.Equals(right);

            public static bool operator !=(Bar left, Bar right)
                => !(left == right);

            public override string ToString()
            {
                return $"{FilledValue}/{TotalValue}";
            }
        }

        public struct YAxisLabels
        {
            public string TopLabel { get; }
            public string MiddleLabel { get; }
            public string BottomLabel { get; }

            public YAxisLabels(string topLabel, string middleLabel, string bottomLabel)
            {
                TopLabel = topLabel;
                MiddleLabel = middleLabel;
                BottomLabel = bottomLabel;
            }

            public static YAxisLabels Empty = new YAxisLabels("", "", "");

            public override bool Equals(object obj)
                => obj is YAxisLabels labels
                && labels.TopLabel == TopLabel
                && labels.MiddleLabel == MiddleLabel
                && labels.BottomLabel == BottomLabel;

            public override int GetHashCode()
                => HashCode.Combine(TopLabel, MiddleLabel, BottomLabel);

            public static bool operator ==(YAxisLabels left, YAxisLabels right)
                => left.Equals(right);

            public static bool operator !=(YAxisLabels left, YAxisLabels right)
                => !(left == right);

            public override string ToString() => $"{TopLabel} {MiddleLabel} {BottomLabel}";
        }
    }
}
