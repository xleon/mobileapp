using System;
using System.Globalization;
using FluentAssertions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Networking.Models.Reports;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;
using Xunit;
using YAxisLabels = Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement.YAxisLabels;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportProjectsBarChartElementTests
    {
        private static readonly double eps = 0.00001;

        public sealed class TheLoadingStateProperty
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToTrue()
            {
                ReportProjectsBarChartElement.LoadingState.IsLoading.Should().BeTrue();
            }
        }

        public sealed class TheConstructor
        {
            private static readonly YAxisLabels expectedLabels = new YAxisLabels("4 h", "2 h", "0 h");

            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                new ReportProjectsBarChartElement(null, DateFormat.ValidDateFormats[0]).IsLoading.Should().BeFalse();
            }
            
            [Fact, LogIfTooSlow]
            public void ConvertsDayReportToBars()
            {
                var timeEntriesTotals = setupTotals(Resolution.Day);
                
                var element = new ReportProjectsBarChartElement(timeEntriesTotals, DateFormat.ValidDateFormats[0]);
                
                element.Bars[0].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                
                element.Bars[1].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);

                element.XLabels.Should().BeEquivalentTo(new string[] { "01/01\nM", "01/02\nT" });
                element.YLabels.Should().BeEquivalentTo(expectedLabels);
            }

            [Fact, LogIfTooSlow]
            public void ConvertsWeeklyReportToBars()
            {
                var timeEntriesTotals = setupTotals(Resolution.Week);
                
                var element = new ReportProjectsBarChartElement(timeEntriesTotals, DateFormat.ValidDateFormats[0]);
                
                element.Bars[0].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                
                element.Bars[1].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);

                element.XLabels.Should().BeEquivalentTo(new string[] { "01/01", "01/08" });
                element.YLabels.Should().BeEquivalentTo(expectedLabels);
            }

            [Fact, LogIfTooSlow]
            public void ConvertsMonthlyReportToBars()
            {
                var timeEntriesTotals = setupTotals(Resolution.Month);
                
                var element = new ReportProjectsBarChartElement(timeEntriesTotals, DateFormat.ValidDateFormats[0]);
                
                element.Bars[0].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                
                element.Bars[1].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);

                element.XLabels.Should().BeEquivalentTo(new string[] { "01/01", "02/01" });
                element.YLabels.Should().BeEquivalentTo(expectedLabels);
            }

            private TimeEntriesTotals setupTotals(Resolution resolution)
            {
                DateFormatCultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                var startDate = new DateTimeOffset(2001, 1, 1, 14, 53, 0, TimeSpan.Zero);
                var endDate = resolution == Resolution.Month ? startDate.AddMonths(1) : startDate.AddDays(7);

                var timeEntriesTotals = new TimeEntriesTotals
                {
                    Resolution = resolution,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var group1 = new TimeEntriesTotalsGroup
                {
                    Billable = TimeSpan.FromHours(2),
                    Total = TimeSpan.FromHours(2)
                };
                
                var group2 = new TimeEntriesTotalsGroup
                {
                    Billable = TimeSpan.FromHours(2),
                    Total = TimeSpan.FromHours(4)
                };
                
                timeEntriesTotals.Groups = new ITimeEntriesTotalsGroup[]
                {
                    group1, group2
                };

                return timeEntriesTotals;
            }
        }
    }
}