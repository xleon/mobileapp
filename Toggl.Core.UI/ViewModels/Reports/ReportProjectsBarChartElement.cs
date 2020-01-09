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
                return Enumerable.Empty<Bar>();

            return report.Groups
                .Select(group => new Bar(group.Billable.TotalHours, group.Total.TotalHours));
        }

        private static IEnumerable<string> convertReportTimeEntriesToXAxisLabels(ITimeEntriesTotals report, DateFormat dateFormat)
        {
            if (report == null)
                yield break;

            if (report.Resolution == Resolution.Day && report.Groups.Length <= 7)
            {
                for (var i = 0; i < report.Groups.Length; i++)
                {
                    var date = report.StartDate.AddDays(i);
                    var dateString = date.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
                    var dayOfWeekString = DateTimeOffsetConversion.ToDayOfWeekInitial(date);
                    yield return $"{dateString}\n{dayOfWeekString}";
                }
                yield break;
            }
            else if (report.Groups.Length == 1)
            {
                yield return report.StartDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
                yield break;
            }

            yield return report.StartDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
            yield return report.EndDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
        }

        private static YAxisLabels convertReportTimeEntriesToYAxisLabels(ITimeEntriesTotals report)
        {
            if (report == null)
                return YAxisLabels.Empty;

            var maxTime = report.Groups.Max(group => group.Total);

            if (maxTime.TotalHours > 1)
            {
                return createLabels(maxTime.TotalHours, Resources.UnitHour);
            }
            else if (maxTime.TotalMinutes > 1)
            {
                return createLabels(maxTime.TotalMinutes, Resources.UnitMin);
            }
            else
            {
                return createLabels(maxTime.TotalSeconds, Resources.UnitSecond);
            }
        }

        private static YAxisLabels createLabels(double maxValue, string unit)
            => new YAxisLabels($"{maxValue:F0} {unit}", $"{maxValue / 2:F0} {unit}", $"0 {unit}");
    }
}
