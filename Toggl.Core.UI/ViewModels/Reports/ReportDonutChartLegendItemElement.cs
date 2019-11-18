using System;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartLegendItemElement : ReportElementBase
    {
        public string Name { get; set; }
        public float Value { get; set; }

        public ReportDonutChartLegendItemElement(ITimeEntriesTotals reportsTotal, ProjectSummaryReport summary)
            : base(false)
        {
            // use the arguments to calculate what's needed for the donut legend item
        }
    }
}
