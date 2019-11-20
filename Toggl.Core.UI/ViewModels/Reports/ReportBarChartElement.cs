using Toggl.Core.Reports;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportBarChartElement : ReportElementBase
    {
        public ReportBarChartElement(ITimeEntriesTotals reportsTotal, ProjectSummaryReport summary)
            : base(false)
        {
            // use the arguments to calculate what's needed for the bar chart
        }

        private ReportBarChartElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportBarChartElement LoadingState
            => new ReportBarChartElement(true);

        // TODO: Do not forget to update this method and write tests for it when the element is implemented
        public override bool Equals(IReportElement other)
            => other is ReportBarChartElement barChartElement
            && barChartElement.IsLoading == IsLoading;
    }
}
