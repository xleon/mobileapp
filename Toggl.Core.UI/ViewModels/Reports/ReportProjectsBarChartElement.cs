using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportProjectsBarChartElement : ReportBarChartElement
    {
        private const int weekLengthInDays = 7;

        public ReportProjectsBarChartElement(ITimeEntriesTotals report, DurationFormat durationFormat)
            : base(convertReportTimeEntriesToBars(report), durationFormat)
        {
        }

        private static IEnumerable<Bar> convertReportTimeEntriesToBars(ITimeEntriesTotals report)
        {
            if (report == null)
            {
                return Enumerable.Empty<Bar>();
            }

            var offsets = convertReportTimeEntriesToOffsets(report);

            return report.Groups
                .Zip(offsets, (group, offset) => new Bar(group.Billable.TotalHours, group.Total.TotalHours, offset));
        }

        private static DateTimeOffsetRange addToDate(DateTimeOffset start, int howMany, Resolution resolution)
        {
            if (resolution == Resolution.Day)
            {
                return new DateTimeOffsetRange(start.AddDays(howMany), start.AddDays(howMany + 1));
            }
            else if (resolution == Resolution.Week)
            {
                return new DateTimeOffsetRange(start.AddDays(weekLengthInDays * howMany),
                    start.AddDays(weekLengthInDays * (howMany + 1)));
            }

            return new DateTimeOffsetRange(start.AddMonths(howMany), start.AddMonths(howMany + 1));
        }

        private static IImmutableList<DateTimeOffsetRange> convertReportTimeEntriesToOffsets(ITimeEntriesTotals report)
            => timeRanges(report.Groups.Length, report.StartDate, report.Resolution);

        private static IImmutableList<DateTimeOffsetRange> timeRanges(int numberOfDays, DateTimeOffset startDate, Resolution resolution)
            => Enumerable.Range(0, numberOfDays)
                .Select(i => addToDate(startDate, i, resolution))
                .ToImmutableList();
    }
}
