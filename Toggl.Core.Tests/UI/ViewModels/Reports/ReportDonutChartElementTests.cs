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
    public sealed class ReportDonutChartElementTests
    {
        public class ReportDonutChartElementsBaseTests
        {
            public class MockDonutElement : ReportDonutChartDonutElement
            {
                public MockDonutElement(ImmutableList<Segment> segments) : base(segments)
                {
                }

                public override bool Equals(IReportElement other)
                    => base.Equals(other) && other is MockDonutElement;
            }

            public class MockLegendItem : ReportDonutChartLegendItemElement
            {
                public MockLegendItem() : base("", "", "", 1)
                {
                }
            }

            protected List<Segment> CreateSegments(
                int count = 5,
                Func<int, double> valueSelector = null,
                Func<int, string> labelSelector = null,
                Func<int, string> colorSelector = null)
            {
                colorSelector ??= _ => "#000000";
                labelSelector ??= x => $"Project-{x + 1}";
                valueSelector ??= x => x + 1;

                return Enumerable.Range(0, count)
                    .Select(i => new Segment(colorSelector(i), labelSelector(i), valueSelector(i)))
                    .ToList();
            }
        }

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

        public sealed class TheConstructor : ReportDonutChartElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                var segments = CreateSegments();
                new ReportDonutChartElement(segments).IsLoading.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void DefaultOverloadCreatesCorrectNumberOfSubElements()
            {
                var count = 5;
                var elements = CreateSegments(count, _ => 1);

                var donutChart = new ReportDonutChartElement(elements);

                donutChart.SubElements.Count.Should().Be(1 + elements.Count);
                donutChart.SubElements.First().Should().BeOfType<ReportDonutChartDonutElement>();
                donutChart.SubElements.Skip(1).Should().AllBeOfType<ReportDonutChartLegendItemElement>();
            }

            [Fact, LogIfTooSlow]
            public void CreatesCorrectDonutElement()
            {
                var segments = CreateSegments();

                var donutChart = new ReportDonutChartElement(segments, s => new MockDonutElement(s.ToImmutableList()));

                var elements = donutChart.SubElements;
                elements.Count.Should().Be(1 + segments.Count);
                elements.First().Should().BeOfType<MockDonutElement>();
                elements.Skip(1).Should().AllBeOfType<ReportDonutChartLegendItemElement>();
            }

            [Fact, LogIfTooSlow]
            public void CreatesCorrectLegendItemElements()
            {
                var segments = CreateSegments();
                var legendItems = segments.Select(_ => new MockLegendItem());
                var donutChart = new ReportDonutChartElement(segments, null, s => legendItems);

                var elements = donutChart.SubElements;
                elements.Count.Should().Be(1 + segments.Count);
                elements.Skip(1).Should().AllBeOfType<MockLegendItem>();
            }
        }

        public sealed class TheEqualsMethod : ReportDonutChartElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForEqualElements()
            {
                var segments = CreateSegments();
                var donutChartA = new ReportDonutChartElement(segments, null, null);
                var donutChartB = new ReportDonutChartElement(segments, null, null);

                donutChartA.Equals(donutChartB).Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInSegments()
            {
                var segmentsA = CreateSegments(5);
                var segmentsB = CreateSegments(3);
                var donutChartA = new ReportDonutChartElement(segmentsA, null, null);
                var donutChartB = new ReportDonutChartElement(segmentsB, null, null);

                donutChartA.Equals(donutChartB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInIsLoading()
            {
                var segmentsA = CreateSegments();
                var donutChartA = new ReportDonutChartElement(segmentsA, null, null);
                var donutChartB = ReportDonutChartElement.LoadingState;

                donutChartA.Equals(donutChartB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInDonutPart()
            {
                var segments = CreateSegments();
                var donutChartA = new ReportDonutChartElement(segments, segments => new MockDonutElement(segments.ToImmutableList()), null);
                var donutChartB = new ReportDonutChartElement(segments);

                donutChartA.Equals(donutChartB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInLegendItemsPart()
            {
                var segments = CreateSegments();
                var donutChartA = new ReportDonutChartElement(segments, null, segments => segments.Select(s => new MockLegendItem()));
                var donutChartB = new ReportDonutChartElement(segments);

                donutChartA.Equals(donutChartB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInType()
            {
                var segments = CreateSegments().ToList();
                var donutChart = new ReportDonutChartElement(segments, segments => new MockDonutElement(segments.ToImmutableList()), null);
                var reportElement = new ReportErrorElement(new Exception());

                donutChart.Equals(reportElement).Should().BeFalse();
            }
        }
    }
}
