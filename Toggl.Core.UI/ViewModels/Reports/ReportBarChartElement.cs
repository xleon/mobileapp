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
    }
}
