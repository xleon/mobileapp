namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartLegendItemElement : ReportElementBase
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Color { get; private set; }
        public double Percentage { get; private set; }

        public ReportDonutChartLegendItemElement(string name, string color, string value, double percentage)
            : base(false)
        {
            Name = name;
            Color = color;
            Value = value;
            Percentage = percentage;
        }

        public override bool Equals(IReportElement other)
            => other is ReportDonutChartLegendItemElement donutChartLegendItemElement
            && donutChartLegendItemElement.IsLoading == IsLoading
            && donutChartLegendItemElement.Name == Name
            && donutChartLegendItemElement.Value == Value
            && donutChartLegendItemElement.Percentage == Percentage;
    }
}
