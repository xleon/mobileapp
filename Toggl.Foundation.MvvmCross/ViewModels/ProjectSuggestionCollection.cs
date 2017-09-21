using System.Collections.Generic;
using System.Collections.ObjectModel;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Multivac;
using static Toggl.Multivac.Extensions.ObservableCollectionExtensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class ProjectSuggestionCollection : ObservableCollection<ProjectSuggestion>
    {
        public string Workspace { get; }

        public ProjectSuggestionCollection(string workspace, IEnumerable<ProjectSuggestion> suggestions)
        {
            Ensure.Argument.IsNotNull(workspace, nameof(workspace));
            Ensure.Argument.IsNotNull(suggestions, nameof(suggestions));

            Workspace = workspace;
            Add(ProjectSuggestion.NoProject);
            this.AddRange(suggestions);
        }
    }
}
