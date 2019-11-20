namespace Toggl.Core.UI.ViewModels.Reports
{
    public abstract class ReportElementBase : IReportElement
    {
        public bool IsLoading { get; }

        public ReportElementBase(bool isLoading = false)
        {
            IsLoading = isLoading;
        }

        public abstract bool Equals(IReportElement other);
    }
}
