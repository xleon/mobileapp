using System.Collections.Generic;
using System.Linq;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions
{
    public sealed class ProjectSuggestionViewModel : BaseTimeEntrySuggestionViewModel
    {
        internal static IEnumerable<ProjectSuggestionViewModel> FromProjects(
            IEnumerable<IDatabaseProject> projects
        ) => projects.Select(p => new ProjectSuggestionViewModel(p));

        public long ProjectId { get; }

        public int NumberOfTasks { get; } = 0;

        public string ProjectName { get; } = "";

        public string ClientName { get; } = "";

        public string ProjectColor { get; }

        public ProjectSuggestionViewModel(IDatabaseProject project)
        {
            ProjectId = project.Id;
            ProjectName = project.Name;
            ProjectColor = project.Color;

            if (project.Client == null) return;

            ClientName = project.Client.Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ProjectName.GetHashCode() * 397) ^
                       (ProjectColor.GetHashCode() * 397) ^
                       ClientName.GetHashCode();
            }
        }
    }
}
