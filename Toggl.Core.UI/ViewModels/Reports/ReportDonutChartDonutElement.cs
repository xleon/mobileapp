using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartDonutElement : ReportElementBase
    {
        public ImmutableList<Segment> Segments { get; } = ImmutableList<Segment>.Empty; 

        public ReportDonutChartDonutElement(ImmutableList<Segment> segments)
            : base(false)
        {
            Segments = segments;
        }

        private ReportDonutChartDonutElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportDonutChartDonutElement LoadingState
            => new ReportDonutChartDonutElement(true);

        public override bool Equals(IReportElement other)
            => other is ReportDonutChartDonutElement donutChartDonutElement
            && donutChartDonutElement.IsLoading == IsLoading
            && donutChartDonutElement.Segments.SequenceEqual(Segments);
    }
}

