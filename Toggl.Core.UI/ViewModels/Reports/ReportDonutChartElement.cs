using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;
using Toggl.Shared;
using System;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartElement : CompositeReportElement
    {
        private IReportElement donutElement;
        private ImmutableList<IReportElement> legend;

        /// <summary>
        /// Elements with larger than the fraction specified by this constant
        /// are included in the chart as individual slices.
        /// </summary>
        public static readonly double GuaranteedSliceThreshold = 0.05;

        /// <summary>
        /// Group into 'Other' all elements that would fill less of
        /// a chart fraction than this constant specifies
        /// </summary>
        public static readonly double GuaranteedForOtherThreshold = 0.01;

        /// <summary>
        /// After all segments larger than 5% (GuaranteedSliceThreshold) are set apart as individual slices
        /// and all segments smaller than 1% (GuaranteedForOtherThreshold) are grouped into Other
        /// the rest are added into the Other category one by one from smallest to larger until the
        /// Other category reaches this size (in this case 5%).
        /// </summary>
        public static readonly double BaseMaximumOtherPercentage = 0.05;

        /// <summary>
        /// The label used for the group of the smaller elements
        /// </summary>
        public static readonly string OtherCategoryLabel = Resources.Other;

        /// <summary>
        /// The color of the Other category segment
        /// </summary>
        public static readonly string OtherCategoryColor = "#808080";

        public ReportDonutChartElement(
            IEnumerable<Segment> segments,
            Func<IEnumerable<Segment>, ReportDonutChartDonutElement> donutSelector = null,
            Func<IEnumerable<Segment>, IEnumerable<ReportDonutChartLegendItemElement>> legendItemsSelector = null)
        {
            segments = normalizeSegments(segments);

            donutSelector = donutSelector ?? defaultDonutSelector;
            donutElement = donutSelector(segments);

            legendItemsSelector = legendItemsSelector ?? defaultLegendItemsSelector;
            legend = legendItemsSelector(segments).Cast<IReportElement>().ToImmutableList();
            
            SubElements = legend.Prepend(donutElement)
                .ToImmutableList();
        }

        private static IEnumerable<ReportDonutChartLegendItemElement> defaultLegendItemsSelector(IEnumerable<Segment> segments)
            => segments.Select(s => new ReportDonutChartLegendItemElement(s.Label, s.Color, s.Value.ToString()));

        private static ReportDonutChartDonutElement defaultDonutSelector(IEnumerable<Segment> segments)
            => new ReportDonutChartDonutElement(segments.ToImmutableList());

        private ReportDonutChartElement(bool isLoading)
            : base(isLoading)
        {
            SubElements = ReportDonutChartDonutElement.LoadingState
                .Yield()
                .Cast<IReportElement>()
                .ToImmutableList();
        }

        public static ReportDonutChartElement LoadingState
            => new ReportDonutChartElement(true);

        /// <summary>
        /// This function normalizes the values into percentages [0,1] and groups
        /// smaller items below a threshold into a separate "Other" item.
        /// This function operates with whatever values come in,
        /// they don't need to be already normalized.
        /// </summary>
        private ImmutableList<Segment> normalizeSegments(IEnumerable<Segment> segments)
        {
            var slices = new List<Segment>();
            var other = new List<Segment>();
            var otherPercentage = 0.0;

            var total = segments.Sum(s => s.Value);

            if (total == 0)
                return ImmutableList<Segment>.Empty;

            foreach (var segment in segments.OrderBy(s => s.Value))
            {
                var percentage = segment.Value / total;

                if (percentage >= GuaranteedSliceThreshold)
                {
                    slices.Add(segment);
                    continue;
                }

                if (percentage <= GuaranteedForOtherThreshold || otherPercentage <= BaseMaximumOtherPercentage)
                {
                    otherPercentage += percentage;
                    other.Add(segment);
                    continue;
                }

                slices.Add(segment);
            }

            if (other.Count == 1)
            {
                slices.Add(other.Single());
                other.Clear();
            }

            // The elements were being added in ascending order, but the final list has to be descending
            slices.Reverse();

            if (other.Any())
            {
                var totalInOther = other.Sum(s => s.Value);
                slices.Add(new Segment(OtherCategoryColor, OtherCategoryLabel, totalInOther));
            }

            return slices.ToImmutableList();
        }

        // TODO: Do not forget to update this method and write tests for it when the element is implemented
        public override bool Equals(IReportElement other)
            => other is ReportDonutChartElement donutChartElement
            && donutChartElement.IsLoading == IsLoading
            && donutChartElement.donutElement.Equals(donutElement)
            && donutChartElement.legend.SequenceEqual(legend);

        public struct Segment
        {
            public string Color { get; }
            public string Label { get; }
            public double Value { get; }

            public Segment(string color, string label, double value)
            {
                Color = color;
                Label = label;
                Value = value;
            }

            public override bool Equals(object obj)
                => obj is Segment segment
                && segment.Color == Color
                && segment.Label == Label
                && segment.Value == Value;

            public override int GetHashCode()
                => HashCode.Combine(Color, Label, Value);

            public static bool operator ==(Segment left, Segment right)
                => left.Equals(right);

            public static bool operator !=(Segment left, Segment right)
                => !(left == right);
        }
    }
}
