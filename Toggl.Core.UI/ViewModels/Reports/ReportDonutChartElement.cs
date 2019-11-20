using System.Linq;
using System.Collections.Immutable;
using Toggl.Core.Reports;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportDonutChartElement : CompositeReportElement
    {
        private ReportDonutChartDonutElement donutElement;
        private IReportElement[] legend;

        public ReportDonutChartElement(ITimeEntriesTotals reportsTotal, ProjectSummaryReport summary)
            : base(false)
        {
            // use the arguments to calculate what's needed for the donut
            donutElement = new ReportDonutChartDonutElement(reportsTotal, summary);

            // use the arguments to calculate the items for the donut legend
            legend = new IReportElement[]
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

        // TODO: Do not forget to update this method and write tests for it when the element is implemented
        public override bool Equals(IReportElement other)
            => other is ReportDonutChartElement donutChartElement
            && donutChartElement.IsLoading == IsLoading
            && donutChartElement.donutElement.Equals(donutElement)
            && donutChartElement.legend.SequenceEqual(legend);
    }
}
