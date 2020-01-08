
namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportWorkspaceNameElement : IReportElement
    {
        public string Name { get; }

        public ReportWorkspaceNameElement(string name)
        {
            Name = name;
        }

        public bool Equals(IReportElement other)
            => GetType() == other.GetType()
            && other is ReportWorkspaceNameElement workspaceNameElement
            && workspaceNameElement.Name == Name;
    }
}
