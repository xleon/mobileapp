namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportNoDataElement : IReportElement
    {
        public bool Equals(IReportElement other)
            => other is ReportNoDataElement;
    }
}
