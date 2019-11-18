using System;
using Toggl.Core.Reports;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class SummaryReportElement : IReportElement
    {
        public float BillablePercentage { get; }
        public TimeSpan TotalTime { get; }

        public SummaryReportElement(ProjectSummaryReport report)
        {
            TotalTime = TimeSpan.FromSeconds(report?.TotalSeconds ?? 0);
            BillablePercentage = report?.BillablePercentage ?? 0;
        }
    }
}