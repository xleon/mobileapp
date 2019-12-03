using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Conversions;
using Toggl.Core.UI.Helper;
using Toggl.Shared;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportProjectsBarChartElement : ReportBarChartElement
    {
        public ReportProjectsBarChartElement(ITimeEntriesTotals report, DateFormat dateFormat)
            : base(convertReportTimeEntriesToBars(report), convertReportTimeEntriesToXAxisLabels(report, dateFormat), convertReportTimeEntriesToYAxisLabels(report))
        {
        }

        private static IEnumerable<Bar> convertReportTimeEntriesToBars(ITimeEntriesTotals report)
        {
            if (report == null)
            {
                return Enumerable.Empty<Bar>();
            }

            return report.Groups
                .Select(group => new Bar(group.Billable.TotalHours, group.Total.TotalHours));
        }

        private static IEnumerable<string> convertReportTimeEntriesToXAxisLabels(ITimeEntriesTotals report, DateFormat dateFormat)
        {
            if(report == null)
            {
                yield break;
            }

            if (report.Resolution == Resolution.Day && report.Groups.Count() <= 7)
            {
                for (var i = 0; i < report.Groups.Count(); i++)
                {
                    var date = report.StartDate.AddDays(i);
                    var dateString = date.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
                    var dayOfWeekString = DateTimeOffsetConversion.ToDayOfWeekInitial(date);
                    yield return $"{dateString}\n{dayOfWeekString}";
                }
                yield break;
            }

            yield return report.StartDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
            yield return report.EndDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
        }

        private static YAxisLabels convertReportTimeEntriesToYAxisLabels(ITimeEntriesTotals report)
        {
            if(report == null)
            {
                return YAxisLabels.Empty;
            }

            Func<double, string> formatTime = (double x) =>
            {
                var asString = x.ToString("F0");
                if (asString.EndsWith(".0", StringComparison.InvariantCulture))
                    return asString[0..^2];
                return asString;
            };

            var maxTime = report.Groups.Max(group => group.Total);
            double maxValue, halfValue;
            string timeUnitString;
            if (maxTime.TotalHours > 1)
            {
                maxValue = maxTime.TotalHours;
                halfValue = maxValue / 2;
                timeUnitString = Resources.UnitHour;
            }
            else if (maxTime.TotalMinutes > 1)
            {
                maxValue = maxTime.TotalMinutes;
                halfValue = maxValue / 2;
                timeUnitString = Resources.UnitMin;
            }
            else
            {
                maxValue = maxTime.TotalSeconds;
                halfValue = maxValue / 2;
                timeUnitString = Resources.UnitSecond;
            }

            return new YAxisLabels(
                $"{formatTime(maxValue)} {timeUnitString}",
                $"{formatTime(halfValue)} {timeUnitString}",
                $"0 {timeUnitString}");
        }
    }
}
