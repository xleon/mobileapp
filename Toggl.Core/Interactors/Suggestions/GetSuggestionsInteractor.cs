using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Suggestions;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Interactors.Suggestions
{
    public sealed class GetSuggestionsInteractor : IInteractor<IObservable<IEnumerable<Suggestion>>>
    {
        private readonly int suggestionCount;
        private readonly IInteractor<IObservable<IReadOnlyList<ISuggestionProvider>>> getsuggestionProvidersInteractor;

        public GetSuggestionsInteractor(
            int suggestionCount,
            IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsInClosedRange(suggestionCount, 1, 9, nameof(suggestionCount));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.getsuggestionProvidersInteractor = interactorFactory.GetSuggestionProviders(suggestionCount);
            this.suggestionCount = suggestionCount;
        }

        public IObservable<IEnumerable<Suggestion>> Execute()
            => getsuggestionProvidersInteractor
                .Execute()
                .SelectMany(CommonFunctions.Identity)
                .Select(provider => provider.GetSuggestions())
                .SelectMany(CommonFunctions.Identity)
                .ToList()
                .SelectMany(removingDuplicates)
                .Take(suggestionCount)
                .ToList();

        private IList<Suggestion> removingDuplicates(IList<Suggestion> suggestions)
            => suggestions
                .GroupBy(s => new { s.Description, s.ProjectId, s.TaskId, s.WorkspaceId })
                .Select(group => group.First())
                .ToList();
    }
}
