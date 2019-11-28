using System;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Toggl.Core.Reports;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;
using System.Collections.Generic;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public class ReportDonutElementsBaseTests
    {
        public class MockDonutElement : ReportDonutChartDonutElement
        {
            public MockDonutElement(ImmutableList<Segment> segments) : base(segments)
            {
            }
        }

        public class MockLegendItem : ReportDonutChartLegendItemElement
        {
            public MockLegendItem() : base("", "", "", 1)
            {
            }
        }

        protected IEnumerable<Segment> CreateSegments(
            int count,
            Func<int, double> valueSelector = null,
            Func<int, string> labelSelector = null,
            Func<int, string> colorSelector = null)
        {
            colorSelector ??= _ => "#000000";
            labelSelector ??= x => $"Project-{x + 1}";
            valueSelector ??= x => x + 1;

            return Enumerable.Range(0, count)
                .Select(i => new Segment(colorSelector(i), labelSelector(i), valueSelector(i)));
        }

        protected IEnumerable<Segment> CreateSegments(
            double[] values,
            Func<int, string> labelSelector = null,
            Func<int, string> colorSelector = null)
            => CreateSegments(values.Length, i => values[i], labelSelector, colorSelector);
    }

    public sealed class ReportDonutChartElementTests
    {
        public sealed class TheLoadingStateProperty
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToTrue()
            {
                ReportDonutChartElement.LoadingState.IsLoading.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void HasOneLoadingSubElement()
            {
                var subElements = ReportDonutChartElement.LoadingState.SubElements;
                var subElement = subElements.OfType<ReportDonutChartDonutElement>().Single();

                subElements.Count.Should().Be(1);
                subElement.IsLoading.Should().BeTrue();
            }
        }

        public sealed class TheConstructor : ReportDonutElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                var segments = CreateSegments(1);
                new ReportDonutChartElement(segments).IsLoading.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void DefaultOverloadCreatesCorrectNumberOfSubElements()
            {
                var count = 4;
                var elements = CreateSegments(count, _ => 1).ToList();

                var donutChart = new ReportDonutChartElement(elements);

                donutChart.SubElements.Count.Should().Be(1 + elements.Count);
                donutChart.SubElements.First().Should().BeOfType<ReportDonutChartDonutElement>();
                donutChart.SubElements.Skip(1).Should().AllBeOfType<ReportDonutChartLegendItemElement>();
            }

            [Fact, LogIfTooSlow]
            public void DefaultOverloadCreatesCorrectNumberOfSubElementsWithOtherCategory()
            {
                var segments = CreateSegments(new[] { 40.0, 30, 2, 15, 5, 7, 1 }).ToList();
                var expectedValues = new[] { 40, 30, 15, 7, 5, 3 };

                var donutChart = new ReportDonutChartElement(segments);

                var elements = donutChart.SubElements;
                var legend = elements.Skip(1).Cast<ReportDonutChartLegendItemElement>();
                elements.Count.Should().Be(1 + expectedValues.Length);
                elements.First().Should().BeOfType<ReportDonutChartDonutElement>();
                legend.Select(li => double.Parse(li.Value)).Should().BeSequenceEquivalentTo(expectedValues);
                var otherLegendItem = legend.Last();
                otherLegendItem.Color.Should().Be(OtherCategoryColor);
                otherLegendItem.Name.Should().Be(OtherCategoryLabel);
            }

            [Fact, LogIfTooSlow]
            public void CreatesCorrectDonutElement()
            {
                var segments = CreateSegments(1).ToList();

                var donutChart = new ReportDonutChartElement(segments, s => new MockDonutElement(s.ToImmutableList()));

                var elements = donutChart.SubElements;
                elements.Count.Should().Be(1 + segments.Count);
                elements.First().Should().BeOfType<MockDonutElement>();
                elements.Skip(1).Should().AllBeOfType<ReportDonutChartLegendItemElement>();
            }

            [Fact, LogIfTooSlow]
            public void CreatesCorrectLegendItemElements()
            {
                var segments = CreateSegments(5).ToList();
                var legendItems = segments.Select(_ => new MockLegendItem());
                var donutChart = new ReportDonutChartElement(segments, null, s => legendItems);

                var elements = donutChart.SubElements;
                elements.Count.Should().Be(1 + segments.Count);
                elements.Skip(1).Should().AllBeOfType<MockLegendItem>();
            }
        }

        public sealed class TheGroupingAlgorithm : ReportDonutElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void GivesEachSegmentOverFivePercentTheirOwnSlice()
            {
                var segments = CreateSegments(new[] { 30.0, 20, 15, 12, 10, 8, 5 }).ToList();

                var donutChart = new ReportDonutChartElement(segments);

                donutChart.SubElements
                    .OfType<ReportDonutChartLegendItemElement>()
                    .Should().HaveCount(segments.Count);
            }

            [Fact, LogIfTooSlow]
            public void GroupsAllSegmentsBelowOnePercentUnderOther()
            {
                var values = Enumerable.Range(0, 101).Select(x => 1.0).ToArray();
                var segments = CreateSegments(values).ToList();
                var sum = values.Sum();

                var donutChart = new ReportDonutChartElement(segments);

                var legend = donutChart.SubElements.OfType<ReportDonutChartLegendItemElement>();
                legend.Should().HaveCount(1);
                legend.Last().Name.Should().Be(OtherCategoryLabel);
                legend.Last().Value.Should().Be(sum.ToString());
            }

            [Fact, LogIfTooSlow]
            public void GroupsUnderOtherUntilOtherIsLargerThan5Percent()
            {
                var values = new[]
                {
                    91.0, // gets its own slice automatically
                    1, 1, 1, 1, // autogroups under Other slice
                    2, // gets into Other because it is not yet filled
                    3, // gets its own slice because the Other group is full
                };
                var segments = CreateSegments(values).ToList();
                var autogrouped = 1 + 1 + 1 + 1;
                var expectedOtherValue = autogrouped + 2;

                var donutChart = new ReportDonutChartElement(segments);

                var legend = donutChart.SubElements.OfType<ReportDonutChartLegendItemElement>();
                legend.Should().HaveCount(3);
                legend.Last().Name.Should().Be(OtherCategoryLabel);
                legend.Last().Value.Should().Be(expectedOtherValue.ToString());
            }

            [Fact, LogIfTooSlow]
            public void UnwrapsSingleOtherElement()
            {
                var segments = CreateSegments(new[] { 99.0, 1 }).ToList();

                var donutChart = new ReportDonutChartElement(segments);

                var legend = donutChart.SubElements.OfType<ReportDonutChartLegendItemElement>();
                legend.Should().HaveCount(2);
                legend.Last().Name.Should().NotBe(OtherCategoryLabel);
            }
        }
    }
}
