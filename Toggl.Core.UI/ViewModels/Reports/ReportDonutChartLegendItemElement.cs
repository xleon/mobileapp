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

        // TODO: Do not forget to update this method and write tests for it when the element is implemented
        public override bool Equals(IReportElement other)
            => other is ReportDonutChartLegendItemElement donutChartLegendItemElement
            && donutChartLegendItemElement.IsLoading == IsLoading
            && donutChartLegendItemElement.Name == Name
            && donutChartLegendItemElement.Value == Value;
    }
}
