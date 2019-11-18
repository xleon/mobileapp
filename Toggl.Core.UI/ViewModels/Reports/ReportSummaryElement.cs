using Toggl.Core.Reports;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportSummaryElement : ReportElementBase
    {
        public ReportSummaryElement(ITimeEntriesTotals reportsTotal, ProjectSummaryReport summary) : this(false)
        {
            // use the arguments to calculate the totals and the billable
        }

        private ReportSummaryElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportSummaryElement LoadingState
            => new ReportSummaryElement(true);
    }
}
