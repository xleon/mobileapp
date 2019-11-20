using System;
using Toggl.Core.Reports;
using Toggl.Shared.Models.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportSummaryElement : ReportElementBase
    {
        public float BillablePercentage { get; }
        public TimeSpan TotalTime { get; }

        public ReportSummaryElement(ProjectSummaryReport summary) : this(false)
        {
            // use the arguments to calculate the totals and the billable
            TotalTime = TimeSpan.FromSeconds(summary?.TotalSeconds ?? 0);
            BillablePercentage = summary?.BillablePercentage ?? 0;
        }

        private ReportSummaryElement(bool isLoading)
            : base(isLoading)
        {
        }

        public static ReportSummaryElement LoadingState
            => new ReportSummaryElement(true);

        public override bool Equals(IReportElement other)
            => other is ReportSummaryElement summaryElement
            && summaryElement.IsLoading == IsLoading
            && summaryElement.BillablePercentage == BillablePercentage
            && summaryElement.TotalTime == TotalTime;
    }
}
