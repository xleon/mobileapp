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
        public DurationFormat DurationFormat { get; }

        public ReportBarChartElement(
            IEnumerable<Bar> bars,
            DurationFormat durationFormat,
            Func<Bar, Bar> scalingFunction = null)
            : base(false)
        {
            var barsList = bars.ToList();
            var upperLimit = upperValueLimit(barsList);

            scalingFunction ??= (bar => normalizeBar(bar, upperLimit));

            Bars = barsList
                .Select(scalingFunction)
                .ToImmutableList();

            DurationFormat = durationFormat;
        }

        private ReportBarChartElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportBarChartElement LoadingState
            => new ReportBarChartElement(true);

        private double upperValueLimit(List<Bar> bars) => bars.Count > 0 ? bars.Max(bar => bar.TotalValue) : 0;

        private Bar normalizeBar(Bar bar, double maxValue) => bar.Scaled(maxValue);

        public override bool Equals(IReportElement other)
            => other is ReportBarChartElement barChartElement
               && barChartElement.IsLoading == IsLoading
               && barChartElement.DurationFormat == DurationFormat
               && barChartElement.Bars.SequenceEqual(Bars);

        public struct Bar
        {
            public double FilledValue { get; }
            public double TotalValue { get; }
            public DateTimeOffsetRange DataTimeRange { get; }

            public Bar(double filledValue, double totalValue, DateTimeOffsetRange offsetRange)
            {
                FilledValue = filledValue;
                TotalValue = totalValue;
                DataTimeRange = offsetRange;
            }

            public Bar Scaled(double maxValue) => new Bar(FilledValue / maxValue, TotalValue / maxValue, DataTimeRange);

            public override bool Equals(object obj)
                => obj is Bar bar
                   && bar.FilledValue == FilledValue
                   && bar.TotalValue == TotalValue
                   && bar.DataTimeRange.Equals(DataTimeRange);

            public override int GetHashCode()
                => HashCode.Combine(FilledValue, TotalValue, DataTimeRange);

            public static bool operator ==(Bar left, Bar right)
                => left.Equals(right);

            public static bool operator !=(Bar left, Bar right)
                => !(left == right);
        }
    }
}