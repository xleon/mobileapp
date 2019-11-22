using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;
using Toggl.Shared;
using System;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartElement : CompositeReportElement
    {
        private IReportElement donutElement;
        private ImmutableList<IReportElement> legend;

        /// <summary>
        /// The maximum percentage of elements under the Other category.
        /// </summary>
        private readonly double groupUnderOtherThreshold = 0.1;

        /// <summary>
        /// The label used for the group of the smaller elements
        /// </summary>
        private readonly string otherCategoryLabel = Resources.Other;

        /// <summary>
        /// The color of the Other category segment
        /// </summary>
        private readonly string otherCategoryColor = "#808080";

        public ReportDonutChartElement(
            IEnumerable<Segment> segments,
            Func<IEnumerable<Segment>, ReportDonutChartDonutElement> donutSelector = null,
            Func<IEnumerable<Segment>, IEnumerable<ReportDonutChartLegendItemElement>> legendItemsSelector = null)
        {
            segments = normalizeSegments(segments);

            donutSelector = donutSelector ?? defaultDonutSelector;
            donutElement = donutSelector(segments);

            // use the arguments to calculate the items for the donut legend
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
            var percentageUsed = 0d;
            var nonGroupedPercentage = 1 - groupUnderOtherThreshold;
            var total = segments.Sum(s => s.Value);

            var normalizedSegments = segments
                .Select(s => s.Normalized(s.Value / total))
                .OrderByDescending(s => s.NormalizedValue)
                .ToList();

            var segmentsWithOther = new List<Segment>();
            for (var i = 0; i < normalizedSegments.Count; i++)
            {
                var segment = normalizedSegments[i];
                percentageUsed += segment.NormalizedValue;

                segmentsWithOther.Add(segment);

                if (percentageUsed >= nonGroupedPercentage)
                {
                    if (i < normalizedSegments.Count - 1)
                        segmentsWithOther.Add(new Segment(otherCategoryColor, otherCategoryLabel, 1 - percentageUsed));

                    break;
                }
            }

            return segmentsWithOther.ToImmutableList();
        }

        // TODO: Do not forget to update this method and write tests for it when the element is implemented
        public override bool Equals(IReportElement other)
            => other is ReportDonutChartElement donutChartElement
            && donutChartElement.IsLoading == IsLoading
            && donutChartElement.donutElement.Equals(donutElement)
            && donutChartElement.legend.SequenceEqual(legend);

        public struct Segment
        {
            public string Color { get; private set; }
            public string Label { get; private set; }
            public double Value { get; private set; }
            public double NormalizedValue { get; private set; }

            public Segment(string color, string label, double value)
            {
                Color = color;
                Label = label;
                Value = value;
                NormalizedValue = value;
            }

            private Segment(string color, string label, double value, double normalizedValue) : this(color, label, value)
            {
                NormalizedValue = normalizedValue;
            }

            public Segment Normalized(double value)
                => new Segment(Color, Label, Value, value);

            public override bool Equals(object obj)
                => obj is Segment segment
                && segment.Color == Color
                && segment.Label == Label
                && segment.Value == Value;

            public override int GetHashCode()
                => HashCode.From(Color, Label, Value);

            public static bool operator ==(Segment left, Segment right)
                => left.Equals(right);

            public static bool operator !=(Segment left, Segment right)
                => !(left == right);
        }
    }
}
