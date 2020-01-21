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
        public partial class ReportDonutChartDonutElementsBaseTests
        {
            protected ImmutableList<Segment> CreateSegments(
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
                    .ToImmutableList();
            }
        }

        public sealed class TheLoadingStateProperty
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToTrue()
            {
                ReportDonutChartDonutElement.LoadingState.IsLoading.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void CreatesAnElementWithNoSegments()
            {
                ReportDonutChartDonutElement.LoadingState.Segments.Should().BeEmpty();
            }
        }

        public sealed class TheConstructor : ReportDonutChartDonutElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                var segments = CreateSegments();
                new ReportDonutChartDonutElement(segments).IsLoading.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void DefaultOverloadSetsSegmentsProperly()
            {
                var count = 5;
                var elements = CreateSegments(count, _ => 1);

                var donutChart = new ReportDonutChartDonutElement(elements);

                donutChart.Segments.Count.Should().Be(elements.Count);
                donutChart.Segments.Should().BeSequenceEquivalentTo(elements);
            }
        }

        public sealed class TheEqualsMethod : ReportDonutChartDonutElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForEqualElements()
            {
                var segments = CreateSegments();
                var donutChartA = new ReportDonutChartDonutElement(segments);
                var donutChartB = new ReportDonutChartDonutElement(segments);

                donutChartA.Equals(donutChartB).Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInSegments()
            {
                var segmentsA = CreateSegments(5);
                var segmentsB = CreateSegments(3);
                var donutChartA = new ReportDonutChartDonutElement(segmentsA);
                var donutChartB = new ReportDonutChartDonutElement(segmentsB);

                donutChartA.Equals(donutChartB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInIsLoading()
            {
                var segmentsA = CreateSegments();
                var donutChartA = new ReportDonutChartDonutElement(segmentsA);
                var donutChartB = ReportDonutChartDonutElement.LoadingState;

                donutChartA.Equals(donutChartB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInType()
            {
                var segments = CreateSegments();
                var donutElement = new ReportDonutChartDonutElement(segments);
                var reportElement = new ReportErrorElement(new Exception());

                donutElement.Equals(reportElement).Should().BeFalse();
            }
        }
    }
}
