using System;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportProjectsDonutChartLegendItemElement : ReportDonutChartLegendItemElement
    {
        public ReportProjectsDonutChartLegendItemElement(string project, string color, TimeSpan duration, DurationFormat durationFormat)
            : base(project, color, duration.ToFormattedString(durationFormat))
        {
        }
    }
}
