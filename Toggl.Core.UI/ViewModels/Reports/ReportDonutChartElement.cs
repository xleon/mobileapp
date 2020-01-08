using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;
using Toggl.Shared;
using System;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public partial class ReportDonutChartElement : CompositeReportElement
    {
        private IReportElement donutElement;
        private ImmutableList<IReportElement> legend;

        public ReportDonutChartElement(
            IEnumerable<Segment> segments,
            Func<IEnumerable<Segment>, ReportDonutChartDonutElement> donutSelector = null,
            Func<IEnumerable<Segment>, IEnumerable<ReportDonutChartLegendItemElement>> legendItemsSelector = null)
        {
            donutSelector = donutSelector ?? defaultDonutSelector;
            donutElement = donutSelector(segments);

            legendItemsSelector = legendItemsSelector ?? defaultLegendItemsSelector;
            legend = legendItemsSelector(segments).Cast<IReportElement>().ToImmutableList();

            SubElements = legend.Prepend(donutElement)
                .ToImmutableList();
        }

        private static IEnumerable<ReportDonutChartLegendItemElement> defaultLegendItemsSelector(IEnumerable<Segment> segments)
        {
            var total = segments.Sum(s => s.Value);

            if (total == 0)
                return Enumerable.Empty<ReportDonutChartLegendItemElement>();

            return segments.Select(s => new ReportDonutChartLegendItemElement(s.Label, s.Color, s.Value.ToString(), s.Value / total));
        }

        private static ReportDonutChartDonutElement defaultDonutSelector(IEnumerable<Segment> segments)
            => new ReportDonutChartDonutElement(segments.ToImmutableList());

        private ReportDonutChartElement(bool isLoading)
            : base(isLoading)
        {
            SubElements = ReportDonutChartDonutElement.LoadingState
                .Yield()
                .Cast<IReportElement>()
                .ToImmutableList();
        }

        public static ReportDonutChartElement LoadingState
            => new ReportDonutChartElement(true);

        public override bool Equals(IReportElement other)
            => GetType() == other.GetType()
            && other is ReportDonutChartElement donutChartElement
            && donutChartElement.IsLoading == IsLoading
            && donutChartElement.donutElement.Equals(donutElement)
            && donutChartElement.legend.SequenceEqual(legend);
    }
}
