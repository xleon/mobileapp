namespace Toggl.Core.UI.ViewModels.Reports
{
    public class ReportDonutChartLegendItemElement : IReportElement
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Color { get; private set; }
        public double Percentage { get; private set; }

        public ReportDonutChartLegendItemElement(string name, string color, string value, double percentage)
        {
            Name = name;
            Color = color;
            Value = value;
            Percentage = percentage;
        }

        public virtual bool Equals(IReportElement other)
            => GetType() == other.GetType()
            && other is ReportDonutChartLegendItemElement donutChartLegendItemElement
            && donutChartLegendItemElement.Name == Name
            && donutChartLegendItemElement.Color == Color
            && donutChartLegendItemElement.Value == Value
            && donutChartLegendItemElement.Percentage == Percentage;
    }
}
