using System.Collections.Immutable;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public abstract class CompositeReportElement : ReportElementBase
    {
        public ImmutableList<IReportElement> SubElements { get; protected set; }

        public CompositeReportElement(bool isLoading = false) : base(isLoading)
        {
            SubElements = ImmutableList<IReportElement>.Empty;
        }
    }
}
