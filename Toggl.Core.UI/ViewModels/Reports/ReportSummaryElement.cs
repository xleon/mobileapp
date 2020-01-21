using System;
using Toggl.Core.Reports;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportSummaryElement : ReportElementBase
    {
        public float? BillablePercentage { get; }
        public TimeSpan? TotalTime { get; }
        public DurationFormat DurationFormat { get; }

        public ReportSummaryElement(ProjectSummaryReport summary, DurationFormat durationFormat) : this(false)
        {
            if (summary != null)
            {
                TotalTime = TimeSpan.FromSeconds(summary.TotalSeconds);
                BillablePercentage = summary.BillablePercentage;
            }

            DurationFormat = durationFormat;
        }

        private ReportSummaryElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportSummaryElement LoadingState
            => new ReportSummaryElement(true);

        public override bool Equals(IReportElement other)
            => GetType() == other.GetType()
            && other is ReportSummaryElement summaryElement
            && summaryElement.IsLoading == IsLoading
            && summaryElement.BillablePercentage == BillablePercentage
            && summaryElement.TotalTime == TotalTime
            && summaryElement.DurationFormat == DurationFormat;
    }
}
