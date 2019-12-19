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
            : base(
                  convertToSegments(summary),
                  segments => donutWithFormattedValue(segments, durationFormat),
                  segments => createLegendItems(segments, durationFormat))
        {
        }

        private static ReportDonutChartDonutElement donutWithFormattedValue(IEnumerable<Segment> segments, DurationFormat durationFormat)
            => new ReportDonutChartDonutElement(
                segments.ToImmutableList(),
                duration => TimeSpan.FromSeconds(duration).ToFormattedString(durationFormat));

        private static IEnumerable<ProjectSegment> convertToSegments(ProjectSummaryReport summary)
            => summary.Segments.Select(segment =>
                new ProjectSegment(segment.Color, segment.ProjectName, segment.ClientName ?? "", segment.TrackedTime.TotalSeconds));

        private static IEnumerable<ReportProjectsDonutChartLegendItemElement> createLegendItems(IEnumerable<Segment> segments, DurationFormat durationFormat)
        {
            var totalValue = segments.Sum(s => s.Value);

            foreach (var segment in segments.OfType<ProjectSegment>())
            {
                var duration = TimeSpan.FromSeconds(segment.Value).ToFormattedString(durationFormat);
                var percentage = 100.0 * (segment.Value / totalValue);

                yield return new ReportProjectsDonutChartLegendItemElement(
                    segment.Label, segment.Color, segment.Client, duration, percentage);
            }
        }

        class ProjectSegment : Segment
        {
            public string Client { get; private set; }

            public ProjectSegment(string color, string project, string client, double value)
                : base(color, project, value)
            {
                Client = client;
            }

            public override bool Equals(object obj)
                => base.Equals(obj)
                && obj is ProjectSegment segment
                && segment.Client == Client;

            public override int GetHashCode()
                => HashCode.Combine(base.GetHashCode(), Client);

            public static bool operator ==(ProjectSegment left, ProjectSegment right)
                => left.Equals(right);

            public static bool operator !=(ProjectSegment left, ProjectSegment right)
                => !(left == right);
        }
    }
}

