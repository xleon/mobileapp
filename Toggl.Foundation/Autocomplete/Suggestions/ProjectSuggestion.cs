using System.Collections.Generic;
using System.Linq;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Autocomplete.Suggestions
{
    public sealed class ProjectSuggestion : AutocompleteSuggestion
    {
        public static ProjectSuggestion NoProject { get; } = new ProjectSuggestion();

        public static IEnumerable<ProjectSuggestion> FromProjects(
            IEnumerable<IDatabaseProject> projects
        ) => projects.Select(p => new ProjectSuggestion(p));

        public static IEnumerable<ProjectSuggestion> FromProjectsPrependingEmpty(
            IEnumerable<IDatabaseProject> projects) 
        {
            yield return NoProject;

            foreach (var project in projects)
                yield return new ProjectSuggestion(project);
        }

        public long ProjectId { get; }

        public int NumberOfTasks { get; } = 0;

        public string ProjectName { get; } = "";

        public string ClientName { get; } = "";

        public string ProjectColor { get; }

        public string Workspace { get; } = "";

        private ProjectSuggestion()
        {
            ProjectId = 0;
            ProjectName = Resources.NoProject;
            ProjectColor = "#A3A3A3";
        }

        public ProjectSuggestion(IDatabaseProject project)
        {
            ProjectId = project.Id;
            ProjectName = project.Name;
            ProjectColor = project.Color;
            Workspace = project.Workspace?.Name;
            ClientName = project.Client?.Name;
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
