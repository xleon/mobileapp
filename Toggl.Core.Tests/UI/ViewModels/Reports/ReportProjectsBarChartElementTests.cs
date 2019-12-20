using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Networking.Models.Reports;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;
using Xunit;
using YAxisLabels = Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement.YAxisLabels;
using static Toggl.Shared.Resolution;

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

            private TimeEntriesTotals setupTotals(Resolution resolution)
            {
                DateFormatCultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var startDate = new DateTimeOffset(2001, 1, 1, 14, 53, 0, TimeSpan.Zero);

                var endDate = resolution == Resolution.Month
                    ? startDate.AddMonths(1)
                    : startDate.AddDays(7);

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

            private TimeEntriesTotals createTotals(Resolution resolution, int groupsCount)
            {
                DateFormatCultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var totals = Substitute.For<ITimeEntriesTotals>();
                totals.Resolution.Returns(resolution);

                var startDate = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
                var endDate = resolution switch
                {
                    Month => startDate.AddMonths(groupsCount),
                    Week => startDate.AddDays(7 * groupsCount),
                    Day => startDate.AddDays(groupsCount),
                    _ => throw new InvalidOperationException()
                };

                var timeEntriesTotals = new TimeEntriesTotals
                {
                    Resolution = resolution,
                    StartDate = startDate,
                    EndDate = endDate
                };

                timeEntriesTotals.Groups = Enumerable.Range(0, groupsCount)
                    .Select(_ => new TimeEntriesTotalsGroup { Billable = TimeSpan.FromHours(1), Total = TimeSpan.FromHours(2) })
                    .ToArray();

                return timeEntriesTotals;
            }

            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                new ReportProjectsBarChartElement(null, DateFormat.ValidDateFormats[0]).IsLoading.Should().BeFalse();
            }

            [Theory, LogIfTooSlow]
            [InlineData(Day, "01/01\nM", "01/02\nT")]
            [InlineData(Week, "01/01", "01/08")]
            [InlineData(Month, "01/01", "02/01")]
            public void ConvertsReportElementToBars(Resolution resolution, string label1, string label2)
            {
                var timeEntriesTotals = setupTotals(resolution);

                var element = new ReportProjectsBarChartElement(timeEntriesTotals, DateFormat.ValidDateFormats[0]);

                element.Bars[0].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].FilledValue.Should().BeApproximately(0.5, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);
                element.XLabels.Should().BeEquivalentTo(new string[] { label1, label2 });
                element.YLabels.Should().BeEquivalentTo(expectedLabels);
            }

            [Theory, LogIfTooSlow]
            [InlineData(Month, 1, 1)]
            [InlineData(Month, 2, 2)]
            [InlineData(Month, 12, 2)]
            [InlineData(Week, 1, 1)]
            [InlineData(Week, 2, 2)]
            [InlineData(Week, 8, 2)]
            [InlineData(Day, 1,1)]
            [InlineData(Day, 2, 2)]
            [InlineData(Day, 5, 5)]
            [InlineData(Day, 7, 7)]
            [InlineData(Day, 8, 2)]
            public void CreatesCorrectAmountOfXAxisLabels(Resolution resolution, int groupsCount, int expectedLabelCount)
            {
                var totals = createTotals(resolution, groupsCount);
                var element = new ReportProjectsBarChartElement(totals, DateFormat.ValidDateFormats.First());

                element.XLabels.Should().HaveCount(expectedLabelCount);
            }
        }
    }
}
