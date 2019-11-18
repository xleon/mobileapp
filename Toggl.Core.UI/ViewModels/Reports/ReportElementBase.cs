using System.Collections.Immutable;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public abstract class ReportElementBase : IReportElement
    {
        public bool IsLoading { get; }

        public ReportElementBase(bool isLoading = false)
        {
            IsLoading = isLoading;
        }
    }
}
