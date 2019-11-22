using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportProjectsDonutChartElement : ReportDonutChartElement
    {
        public ReportProjectsDonutChartElement(ProjectSummaryReport summary, DurationFormat durationFormat)
            : base(convertToSegments(summary), null, segments => createLegendItems(segments, durationFormat))
        {
        }

        private static IEnumerable<Segment> convertToSegments(ProjectSummaryReport summary)
            => summary.Segments.Select(segment =>
                new Segment(segment.Color, segment.ProjectName, segment.TrackedTime.TotalSeconds));

        private static IEnumerable<ReportProjectsDonutChartLegendItemElement> createLegendItems(IEnumerable<Segment> segments, DurationFormat durationFormat)
            => segments.Select(segment =>
                new ReportProjectsDonutChartLegendItemElement(segment.Label, segment.Color, TimeSpan.FromSeconds(segment.Value), durationFormat));
    }
}

