using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.Extensions;
using Toggl.Shared;

namespace Toggl.Core.Interactors.AutocompleteSuggestions
{
    public class GetAutocompleteSuggestions : IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>>
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly QueryInfo queryInfo;

        public GetAutocompleteSuggestions(IInteractorFactory interactorFactory, QueryInfo queryInfo)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(queryInfo, nameof(queryInfo));

            this.interactorFactory = interactorFactory;
            this.queryInfo = queryInfo;
        }

        public IObservable<IEnumerable<AutocompleteSuggestion>> Execute()
        {
            var wordsToQuery = queryInfo.Text.SplitToQueryWords();
            switch (queryInfo.SuggestionType)
            {
                case AutocompleteSuggestionType.Projects:
                    return interactorFactory.GetProjectsAutocompleteSuggestions(wordsToQuery).Execute();

                case AutocompleteSuggestionType.Tags:
                    return interactorFactory.GetTagsAutocompleteSuggestions(wordsToQuery).Execute();
            }

            return wordsToQuery.Count == 0
                ? Observable.Return(QuerySymbolSuggestion.Suggestions)
                : interactorFactory.GetTimeEntriesAutocompleteSuggestions(wordsToQuery).Execute();
        }
    }
}
