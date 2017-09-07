using System.Collections.Generic;
using System.Linq;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions
{
    public sealed class ProjectSuggestionViewModel : BaseTimeEntrySuggestionViewModel
    {
        private static readonly ProjectSuggestionViewModel empty = new ProjectSuggestionViewModel();

        internal static IEnumerable<ProjectSuggestionViewModel> FromProjects(
            IEnumerable<IDatabaseProject> projects
        ) => projects.Select(p => new ProjectSuggestionViewModel(p));

        internal static IEnumerable<ProjectSuggestionViewModel> FromProjectsPrependingEmpty(
            IEnumerable<IDatabaseProject> projects) 
        {
            yield return empty;

            foreach (var project in projects)
                yield return new ProjectSuggestionViewModel(project);
        }

        public long ProjectId { get; }

        public int NumberOfTasks { get; } = 0;

        public string ProjectName { get; } = "";

        public string ClientName { get; } = "";

        public string ProjectColor { get; }

        private ProjectSuggestionViewModel()
        {
            ProjectId = 0;
            ProjectName = Resources.NoProject;
            ProjectColor = "#A3A3A3";
        }

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
