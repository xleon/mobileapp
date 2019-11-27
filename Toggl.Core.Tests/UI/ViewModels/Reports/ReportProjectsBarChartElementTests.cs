using System;
using System.Globalization;
using FluentAssertions;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Networking.Models.Reports;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;
using Xunit;

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
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                new ReportProjectsBarChartElement(null, DurationFormat.Classic).IsLoading.Should().BeFalse();
            }
            
            [Fact, LogIfTooSlow]
            public void ConvertsDayReportToBars()
            {
                var timeEntriesTotals = setupTotals(Resolution.Day);
                var startDate = timeEntriesTotals.StartDate;
                
                var element = new ReportProjectsBarChartElement(timeEntriesTotals, DurationFormat.Classic);
                
                element.Bars[0].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].DataTimeRange.Should().Be(new DateTimeOffsetRange(startDate, startDate.AddDays(1)));
                
                element.Bars[1].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);
                element.Bars[1].DataTimeRange.Should().Be(new DateTimeOffsetRange(startDate.AddDays(1), startDate.AddDays(2)));
            }
            
            [Fact, LogIfTooSlow]
            public void ConvertsWeeklyReportToBars()
            {
                var timeEntriesTotals = setupTotals(Resolution.Week);
                var startDate = timeEntriesTotals.StartDate;
                
                var element = new ReportProjectsBarChartElement(timeEntriesTotals, DurationFormat.Classic);
                
                element.Bars[0].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].DataTimeRange.Should().Be(new DateTimeOffsetRange(startDate, startDate.AddDays(7)));
                
                element.Bars[1].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);
                element.Bars[1].DataTimeRange.Should().Be(new DateTimeOffsetRange(startDate.AddDays(7), startDate.AddDays(14)));
            }
            
            [Fact, LogIfTooSlow]
            public void ConvertsMonthlyReportToBars()
            {
                var timeEntriesTotals = setupTotals(Resolution.Month);
                var startDate = timeEntriesTotals.StartDate;
                
                var element = new ReportProjectsBarChartElement(timeEntriesTotals, DurationFormat.Classic);
                
                element.Bars[0].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].DataTimeRange.Should().Be(new DateTimeOffsetRange(startDate, startDate.AddMonths(1)));
                
                element.Bars[1].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);
                element.Bars[1].DataTimeRange.Should().Be(new DateTimeOffsetRange(startDate.AddMonths(1), startDate.AddMonths(2)));
            }

            private TimeEntriesTotals setupTotals(Resolution resolution)
            {
                var startDate = new DateTimeOffset(2001, 1, 1, 14, 53, 0, TimeSpan.Zero);

                var timeEntriesTotals = new TimeEntriesTotals
                {
                    Resolution = resolution,
                    StartDate = startDate
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