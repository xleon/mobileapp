using System;
using FluentAssertions;
using Toggl.Core.Reports;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportSummaryElementTests
    {
        public sealed class TheConstructor
        {
            [Fact, LogIfTooSlow]
            public void SetsValuesToNullForNullReportSummary()
            {
                var reportSummaryElement = new ReportSummaryElement(null, DurationFormat.Improved);

                reportSummaryElement.BillablePercentage.Should().Be(null);
                reportSummaryElement.TotalTime.Should().Be(null);
            }

            [Fact, LogIfTooSlow]
            public void UpdatesValuesToTheOnesFromReportSummary()
            {
                var eps = 0.0001f;
                var chartSegment = new ChartSegment("", "", 100, 100, 50, "", DurationFormat.Classic);
                var projectSummary = new ProjectSummaryReport(new[] { chartSegment }, 0);

                var reportSummaryElement = new ReportSummaryElement(projectSummary, DurationFormat.Improved);

                reportSummaryElement.BillablePercentage.Should().BeApproximately(50.0f, eps);
                reportSummaryElement.TotalTime.Should().BeCloseTo(TimeSpan.FromSeconds(100.0f), TimeSpan.FromSeconds(eps));
            }

            [Theory, LogIfTooSlow]
            [InlineData(DurationFormat.Classic)]
            [InlineData(DurationFormat.Improved)]
            [InlineData(DurationFormat.Decimal)]
            public void SetsTheCorrectDurationFormat(DurationFormat expectedFormat)
            {
                var reportSummaryElement = new ReportSummaryElement(null, expectedFormat);

                reportSummaryElement.DurationFormat.Should().Be(expectedFormat);
            }
        }

        public sealed class TheLoadingStateProperty
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToTrue()
            {
                ReportSummaryElement.LoadingState.IsLoading.Should().BeTrue();
            }
        }

        public sealed class TheIsLoadingProperty
        {
            [Fact, LogIfTooSlow]
            public void IsSetToFalseIfDefaultConstructorIsUsed()
            {
                var reportSummaryElement = new ReportSummaryElement(null, DurationFormat.Decimal);

                reportSummaryElement.IsLoading.Should().BeFalse();
            }
        }
    }
}
