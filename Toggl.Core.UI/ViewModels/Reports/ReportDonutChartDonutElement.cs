using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;
using System;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartDonutElement : ReportElementBase
    {
        public ImmutableList<Segment> Segments { get; } = ImmutableList<Segment>.Empty;

        public Func<double, string> ValueFormatter { get; set; }

        public ReportDonutChartDonutElement(ImmutableList<Segment> segments, Func<double, string> valueFormatter = null)
            : base(false)
        {
            Segments = segments;
            ValueFormatter = valueFormatter;
        }

        private ReportDonutChartDonutElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportDonutChartDonutElement LoadingState
            => new ReportDonutChartDonutElement(true);

        public override bool Equals(IReportElement other)
            => GetType() == other.GetType()
            && other is ReportDonutChartDonutElement donutChartDonutElement
            && donutChartDonutElement.IsLoading == IsLoading
            && donutChartDonutElement.Segments.SequenceEqual(Segments);
    }
}

