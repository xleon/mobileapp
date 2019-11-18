using System;
using FluentAssertions;
using FsCheck.Xunit;
using Toggl.Core.Reports;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class SummaryReportElementTests
    {
        public sealed class TheConstructor
        {
            [Property]
            public void SetsValuesToZeroForNullReportSummary()
            {
                var summaryReportElement = new SummaryReportElement(null);

                summaryReportElement.BillablePercentage.Should().Be(0);
                summaryReportElement.TotalTime.Should().Be(TimeSpan.Zero);
            }

            [Property]
            public void UpdatesValuesToTheOnesFromReportSummary()
            {
                var eps = 0.0001f;

                var chartSegment = new ChartSegment("", "", 100, 100, 50, "", DurationFormat.Classic);
                var projectSummary = new ProjectSummaryReport(new[] {chartSegment}, 0);

                var summaryReportElement = new SummaryReportElement(projectSummary);

                summaryReportElement.BillablePercentage.Should().BeApproximately(50.0f, eps);
                summaryReportElement.TotalTime.Should().BeCloseTo(TimeSpan.FromSeconds(100.0f), TimeSpan.FromSeconds(eps));
            }
        }
    }
}