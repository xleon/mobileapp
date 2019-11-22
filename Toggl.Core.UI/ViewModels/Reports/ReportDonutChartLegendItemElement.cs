using System;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models.Reports;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartLegendItemElement : ReportElementBase
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Color { get; private set; }

        public ReportDonutChartLegendItemElement(string name, string color, string value)
            : base(false)
        {
            Name = name;
            Color = color;
            Value = value;
        }

        // TODO: Do not forget to update this method and write tests for it when the element is implemented
        public override bool Equals(IReportElement other)
            => other is ReportDonutChartLegendItemElement donutChartLegendItemElement
            && donutChartLegendItemElement.IsLoading == IsLoading
            && donutChartLegendItemElement.Name == Name
            && donutChartLegendItemElement.Value == Value;
    }
}
