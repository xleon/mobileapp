using System.Collections.Generic;
using System.Linq;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Foundation.MvvmCross.Collections;

namespace Toggl.Foundation.MvvmCross.Helper
{
    internal static class AutocompleteExtensions
    {
        public static IEnumerable<WorkspaceGroupedCollection<AutocompleteSuggestion>> GroupByWorkspaceAddingNoProject(
            this IEnumerable<AutocompleteSuggestion> suggestions)
            => suggestions
                .Where(suggestion => !string.IsNullOrEmpty(suggestion.WorkspaceName))
                .GroupBy(suggestion => (suggestion.WorkspaceId, suggestion.WorkspaceName))
                .Select(collectionWithNoProject);


        private static WorkspaceGroupedCollection<AutocompleteSuggestion> collectionWithNoProject(
            IGrouping<(long workspaceId, string workspaceName), AutocompleteSuggestion> grouping)
        {
            var collection = new WorkspaceGroupedCollection<AutocompleteSuggestion>(
                grouping.Key.workspaceName, grouping);
            collection.Insert(
                0,
                ProjectSuggestion.NoProjectWithWorkspace(
                    grouping.Key.workspaceId,
                    grouping.Key.workspaceName));
            return collection;
        }
    }
}
