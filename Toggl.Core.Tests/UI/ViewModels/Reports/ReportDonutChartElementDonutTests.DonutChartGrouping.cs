using System;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed partial class ReportDonutChartElementDonutTests
    {
        private const double eps = 0.00000001;

        public partial class ReportDonutChartDonutElementsBaseTests
        {
            protected ImmutableList<Segment> CreateSegments(
                double[] values,
                Func<int, string> labelSelector = null,
                Func<int, string> colorSelector = null)
                => CreateSegments(values.Length, i => values[i], labelSelector, colorSelector);
        }

        public sealed class DonutChartGroupingTests : ReportDonutChartDonutElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void GroupsIntoCorrectNumberOfSegmentsWhenNoGroupingIsDone()
            {
                var segments = CreateSegments(4);

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Should().HaveCount(4);
            }

            [Fact, LogIfTooSlow]
            public void GroupsIntoSegmentsInCorrectOrder()
            {
                var segments = CreateSegments(4);

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Select(segment => segment.OriginalPercentage).Should().BeInDescendingOrder();
            }

            [Fact, LogIfTooSlow]
            public void GroupsIntoSegmentsWithCorrectPercentages()
            {
                var segments = CreateSegments(4);
                var total = segments.Sum(s => s.Value);
                var expectedPercentages = segments.Select(s => s.Value / total).OrderByDescending(val => val);

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Select(s => s.OriginalPercentage).Should().BeSequenceEquivalentTo(expectedPercentages);
            }

            [Fact, LogIfTooSlow]
            public void GroupsWithNormalizedPercentagesUntouchedIfNormalizationNotNeeded()
            {
                var segments = CreateSegments(4);

                var percentageSegments = DonutChartGrouping.Group(segments);

                var originalPercentages = percentageSegments.Select(s => s.OriginalPercentage);
                var normalizedPercentages = percentageSegments.Select(s => s.NormalizedPercentage);
                originalPercentages.Should().BeSequenceEquivalentTo(normalizedPercentages);
            }

            [Fact, LogIfTooSlow]
            public void GroupsSegmentsSmallerThanThresholdIntoOtherCategory()
            {
                var smallSegmentsCount = 3;
                var smallSegmentValue = DonutChartGrouping.GuaranteedForOtherThreshold / 2;
                var totalSmallSegmentValues = smallSegmentsCount * smallSegmentValue;
                var bigSlice = 1 - totalSmallSegmentValues;
                var smallSegmentValues = Enumerable.Range(0, smallSegmentsCount).Select(_ => smallSegmentValue);
                var segments = CreateSegments(new[] { bigSlice }.Concat(smallSegmentValues).ToArray());

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Should().HaveCount(2);
                percentageSegments[0].OriginalPercentage.Should().Be(bigSlice);

                percentageSegments[1].Segment.Value.Should().Be(totalSmallSegmentValues);
                percentageSegments[1].Segment.Label.Should().Be(DonutChartGrouping.OtherCategoryLabel);
                percentageSegments[1].Segment.Color.Should().Be(DonutChartGrouping.OtherCategoryColor);
                percentageSegments[1].OriginalPercentage.Should().Be(totalSmallSegmentValues);
            }

            [Fact, LogIfTooSlow]
            public void UnwrapsSingleSegmentSmallerThanThresholdInOtherCategory()
            {
                var segments = CreateSegments(new[] { 99.5, 0.5 });

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Should().HaveCount(2);
                percentageSegments[1].Segment.Label.Should().NotBe(DonutChartGrouping.OtherCategoryLabel);
            }

            [Fact, LogIfTooSlow]
            public void OtherSegmentIsLastEvenIfNotTheSmallest()
            {
                var segments = CreateSegments(new[] { 92.0, 2, 2, 2, 2 });

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Should().HaveCount(3);
                percentageSegments[0].Segment.Label.Should().NotBe(DonutChartGrouping.OtherCategoryLabel);
                percentageSegments[1].Segment.Label.Should().NotBe(DonutChartGrouping.OtherCategoryLabel);
                percentageSegments[2].Segment.Label.Should().Be(DonutChartGrouping.OtherCategoryLabel);
                percentageSegments[2].Segment.Value.Should().BeGreaterThan(percentageSegments[1].Segment.Value);
            }

            [Fact, LogIfTooSlow]
            public void NormalizesSegmentsSoThatTheSmallestSliceHasAtLeastMinimumSize()
            {
                var segments = CreateSegments(new[] { 99.9, 0.05, 0.05 });

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Should().HaveCount(2);
                percentageSegments[0].NormalizedPercentage.Should().BeApproximately(0.99, eps);
                percentageSegments[1].NormalizedPercentage.Should().BeApproximately(0.01, eps);
            }

            [Fact, LogIfTooSlow]
            public void NormalizesSegmentsWithoutAffectingOriginalPercentagesAndValues()
            {
                var segments = CreateSegments(new[] { 99.9, 0.05, 0.05 });

                var percentageSegments = DonutChartGrouping.Group(segments);

                percentageSegments.Should().HaveCount(2);
                percentageSegments[0].OriginalPercentage.Should().BeApproximately(0.999, eps);
                percentageSegments[1].OriginalPercentage.Should().BeApproximately(0.001, eps);
                percentageSegments[0].Segment.Value.Should().BeApproximately(99.9, eps);
                percentageSegments[1].Segment.Value.Should().BeApproximately(0.1, eps);
            }
        }
    }
}
