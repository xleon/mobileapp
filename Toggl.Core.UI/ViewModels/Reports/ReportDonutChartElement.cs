using System.Linq;
using System.Collections.Immutable;
using Toggl.Core.Reports;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportDonutChartElement : CompositeReportElement
    {
        public ReportDonutChartElement(ITimeEntriesTotals reportsTotal, ProjectSummaryReport summary)
            : base(false)
        {
            // use the arguments to calculate what's needed for the donut
            var donutElement = new ReportDonutChartDonutElement(reportsTotal, summary);

            // use the arguments to calculate the items for the donut legend
            var legend = new IReportElement[]
            {
                new ReportDonutChartLegendItemElement(reportsTotal, summary),
                new ReportDonutChartLegendItemElement(reportsTotal, summary),
                new ReportDonutChartLegendItemElement(reportsTotal, summary),
            };

            SubElements = legend.Prepend(donutElement)
                .ToImmutableList();
        }

        private ReportDonutChartElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportDonutChartElement LoadingState
            => new ReportDonutChartElement(true);
    }
}
