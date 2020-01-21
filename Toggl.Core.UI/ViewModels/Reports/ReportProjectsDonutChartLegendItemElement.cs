using System;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportProjectsDonutChartLegendItemElement : ReportDonutChartLegendItemElement
    {
        public string Client { get; private set; }

        public ReportProjectsDonutChartLegendItemElement(string project, string color, string client, string duration, double percentage)
            : base(project, color, duration, percentage)
        {
            Client = client;
        }

        public override bool Equals(IReportElement other)
            => base.Equals(other)
            && other is ReportProjectsDonutChartLegendItemElement legendItemElement
            && legendItemElement.Client == Client;
    }
}
