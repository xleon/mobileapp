using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;
namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportProjectsDonutChartElementTests
    {
        public class ReportProjectsDonutChartElementBaseTests 
        {
            protected IEnumerable<ChartSegment> CreateChartSegments(
               int count,
               Func<int, float> timeTrackedSelector = null,
               Func<int, float> billableTimeSelector = null,
               Func<int, string> projectSelector = null,
               Func<int, string> colorSelector = null,
               Func<int, string> clientSelector = null,
               Func<int, DurationFormat> durationFormatSelector = null)
            {
                timeTrackedSelector ??= _ => 100;
                billableTimeSelector ??= _ => 20;
                projectSelector ??= x => $"Project-{x + 1}";
                colorSelector ??= _ => "#404040";
                clientSelector ??= _ => $"Client";
                durationFormatSelector ??= _ => DurationFormat.Improved;

                return Enumerable.Range(0, count)
                    .Select(i => new ChartSegment(
                        projectSelector(i),
                        clientSelector(i),
                        0,
                        timeTrackedSelector(i),
                        billableTimeSelector(i),
                        colorSelector(i),
                        durationFormatSelector(i)));
            }

            protected IEnumerable<ChartSegment> CreateChartSegments(
               float[] values,
               Func<int, float> billableTimeSelector = null,
               Func<int, string> projectSelector = null,
               Func<int, string> colorSelector = null,
               Func<int, string> clientSelector = null,
               Func<int, DurationFormat> durationFormatSelector = null)
                => CreateChartSegments(
                    values.Length,
                    i => values[i],
                    billableTimeSelector,
                    projectSelector, colorSelector,
                    clientSelector,
                    durationFormatSelector);
        }

        public sealed class TheLoadingStateProperty : ReportProjectsDonutChartElementBaseTests
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToTrue()
            {
                ReportProjectsDonutChartElement.LoadingState.IsLoading.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void CreateAnElementWithNoSegments()
            {
                ReportProjectsDonutChartElement.LoadingState.SubElements.Single().Should().BeOfType<ReportDonutChartDonutElement>();
            }
        }

        public sealed class TheConstructor : ReportProjectsDonutChartElementBaseTests
        {
            [Fact, LogIfTooSlow]
            public void CreatesSegmentsThatAreNotLoading()
            {
                var chartSegments = CreateChartSegments(1).ToArray();
                var summary = new ProjectSummaryReport(chartSegments, 0);

                var donutElement = new ReportProjectsDonutChartElement(summary, DurationFormat.Classic);

                donutElement.SubElements
                    .Take(1)
                    .Cast<ReportDonutChartDonutElement>()
                    .Single()
                    .IsLoading.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void CreatesCorrectElements()
            {
                var chartSegments = CreateChartSegments(1).ToArray();
                var summary = new ProjectSummaryReport(chartSegments, 0);

                var donutElement = new ReportProjectsDonutChartElement(summary, DurationFormat.Classic);

                var subElements = donutElement.SubElements;
                subElements.Should().HaveCount(2);
                subElements.First().Should().BeOfType<ReportDonutChartDonutElement>();
                var legendItems = subElements.Skip(1);
                legendItems.Should().HaveCount(1);
                legendItems.Should().AllBeOfType<ReportProjectsDonutChartLegendItemElement>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(DurationFormat.Classic)]
            [InlineData(DurationFormat.Decimal)]
            [InlineData(DurationFormat.Improved)]
            public void LegendItemsTakeDurationFormatIntoAccount(DurationFormat durationFormat)
            {
                var values = new[] { 100f };
                var chartSegments = CreateChartSegments(values).ToArray();
                var summary = new ProjectSummaryReport(chartSegments, 0);
                var expectedValues = values.Select(value => TimeSpan.FromSeconds(value).ToFormattedString(durationFormat));

                var donutElement = new ReportProjectsDonutChartElement(summary, durationFormat);

                donutElement.SubElements
                    .OfType<ReportProjectsDonutChartLegendItemElement>()
                    .Select(x => x.Value)
                    .Should().BeSequenceEquivalentTo(expectedValues);
            }

            [Fact, LogIfTooSlow]
            public void CreatesLegendWithCorrectValues()
            {
                var durationFormat = DurationFormat.Improved;
                var values = new[] { 40f, 30, 20, 10 };
                var chartSegments = CreateChartSegments(values).ToArray();
                var summary = new ProjectSummaryReport(chartSegments, 0);
                var expectedValues = values.Select(value => TimeSpan.FromSeconds(value).ToFormattedString(durationFormat));

                var donutElement = new ReportProjectsDonutChartElement(summary, durationFormat);

                donutElement.SubElements
                    .OfType<ReportProjectsDonutChartLegendItemElement>()
                    .Select(x => x.Value)
                    .Should().BeSequenceEquivalentTo(expectedValues);
            }

        }
    }
}
