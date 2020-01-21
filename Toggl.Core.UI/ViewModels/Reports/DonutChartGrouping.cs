using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public static class DonutChartGrouping
    {
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

        // <summary>
        /// This function normalizes the values into percentages [0,1] and groups
        /// smaller items below a threshold into a separate "Other" item.
        /// This function operates with whatever values come in,
        /// they don't need to be already normalized.
        /// </summary>
        public static ImmutableList<PercentageDecoratedSegment> Group(IEnumerable<Segment> segments)
        {
            var slices = new List<PercentageDecoratedSegment>();
            var other = new List<Segment>();
            var otherPercentage = 0.0;

            var total = segments.Sum(s => s.Value);

            if (total == 0)
                return ImmutableList<PercentageDecoratedSegment>.Empty;

            foreach (var segment in segments.OrderBy(s => s.Value))
            {
                var percentage = segment.Value / total;

                if (percentage >= GuaranteedSliceThreshold)
                {
                    slices.Add(new PercentageDecoratedSegment(segment, percentage));
                    continue;
                }

                if (percentage <= GuaranteedForOtherThreshold || otherPercentage <= BaseMaximumOtherPercentage)
                {
                    otherPercentage += percentage;
                    other.Add(segment);
                    continue;
                }

                slices.Add(new PercentageDecoratedSegment(segment, percentage));
            }

            unwrapOtherIfSingleElement(slices, other, otherPercentage);

            // The elements were being added in ascending order, but the final list has to be descending
            slices.Reverse();

            addOtherCategoryToSlicesIfNeeded(slices, other, otherPercentage);

            slices = normalizeSlicesIfLastSliceIsTooSmallToDisplay(slices);

            return slices.ToImmutableList();
        }

        private static void unwrapOtherIfSingleElement(List<PercentageDecoratedSegment> slices, List<Segment> other, double otherPercentage)
        {
            if (other.Count == 1)
            {
                slices.Add(new PercentageDecoratedSegment(other.Single(), otherPercentage));
                other.Clear();
            }
        }

        private static void addOtherCategoryToSlicesIfNeeded(List<PercentageDecoratedSegment> slices, List<Segment> other, double otherPercentage)
        {
            if (other.None())
                return;

            var totalInOther = other.Sum(s => s.Value);
            var otherSegment = new Segment(OtherCategoryColor, OtherCategoryLabel, totalInOther);
            slices.Add(new PercentageDecoratedSegment(otherSegment, otherPercentage));
        }

        private static List<PercentageDecoratedSegment> normalizeSlicesIfLastSliceIsTooSmallToDisplay(List<PercentageDecoratedSegment> slices)
        {
            var lastSlice = slices[^1];

            if (lastSlice.OriginalPercentage >= GuaranteedForOtherThreshold)
                return slices;

            var factor = (1 - GuaranteedForOtherThreshold) / (1 - lastSlice.OriginalPercentage);

            return slices
                .Take(slices.Count - 1)
                .Select(slice => slice.WithNormalizedPercentage(slice.OriginalPercentage * factor))
                .Append(lastSlice.WithNormalizedPercentage(GuaranteedForOtherThreshold))
                .ToList();
        }
    }
}
