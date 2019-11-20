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

        // TODO: Do not forget to update this method and write tests for it when the element is implemented
        public override bool Equals(IReportElement other)
            => other is ReportDonutChartDonutElement donutChartDonutElement
            && donutChartDonutElement.IsLoading == IsLoading;
    }
}

