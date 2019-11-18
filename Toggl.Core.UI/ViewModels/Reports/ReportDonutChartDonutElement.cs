using Toggl.Core.Reports;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportDonutChartDonutElement : ReportElementBase
    {
        public ReportDonutChartDonutElement(ITimeEntriesTotals reportsTotal, ProjectSummaryReport summary)
            : base(false)
        {
            // use the arguments to calculate what's needed for the donut segments.
        }

        private ReportDonutChartDonutElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportDonutChartDonutElement LoadingState
            => new ReportDonutChartDonutElement(true);
    }
}
