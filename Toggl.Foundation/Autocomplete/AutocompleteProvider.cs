using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Autocomplete
{
    public sealed class AutocompleteProvider : IAutocompleteProvider
    {
        private readonly ITogglDatabase database;

        public AutocompleteProvider(ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));

            this.database = database;
        }

        public IObservable<IEnumerable<AutocompleteSuggestion>> Query(TextFieldInfo info)
        {
            var (queryText, suggestionType) = parseQuery(info);
            return Query(queryText, suggestionType);
        }

        public IObservable<IEnumerable<AutocompleteSuggestion>> Query(string queryText, AutocompleteSuggestionType suggestionType)
        {
            var wordsToQuery = queryText.Split(' ').Where(word => !string.IsNullOrEmpty(word)).Distinct();
            return querySuggestions(wordsToQuery, suggestionType);
        }

        private (string, AutocompleteSuggestionType) parseQuery(TextFieldInfo info)
        {
            if (string.IsNullOrEmpty(info.Text) || info.ProjectId != null)
                return (info.Text, AutocompleteSuggestionType.TimeEntries);

            var stringToSearch = info.Text.Substring(0, info.DescriptionCursorPosition);
            var indexOfQuerySymbol = stringToSearch.LastIndexOfAny(QuerySymbols.All);
            if (indexOfQuerySymbol >= 0)
            {
                var startingIndex = indexOfQuerySymbol + 1;
                var stringLength = info.Text.Length - indexOfQuerySymbol - 1;
                return (info.Text.Substring(startingIndex, stringLength), AutocompleteSuggestionType.Projects);
            }

            return (info.Text, AutocompleteSuggestionType.TimeEntries);
        }

        private IObservable<IEnumerable<AutocompleteSuggestion>> querySuggestions(
            IEnumerable<string> wordsToQuery, AutocompleteSuggestionType suggestionType)
        {
            var queryListIsEmpty = !wordsToQuery.Any();

            if (suggestionType == AutocompleteSuggestionType.Projects)
            {
                if (queryListIsEmpty)
                    return database.Projects.GetAll()
                        .Select(ProjectSuggestion.FromProjectsPrependingEmpty);

                return wordsToQuery
                    .Aggregate(database.Projects.GetAll(), (obs, word) => obs.Select(filterProjectsByWord(word)))
                    .Select(ProjectSuggestion.FromProjects);
            }

            if (queryListIsEmpty)
                return Observable.Return(Enumerable.Empty<AutocompleteSuggestion>());

            return wordsToQuery
               .Aggregate(database.TimeEntries.GetAll(), (obs, word) => obs.Select(filterTimeEntriesByWord(word)))
               .Select(TimeEntrySuggestion.FromTimeEntries);
        }

        private Func<IEnumerable<IDatabaseTimeEntry>, IEnumerable<IDatabaseTimeEntry>> filterTimeEntriesByWord(string word)
            => timeEntries =>
                timeEntries.Where(
                    te => te.Description.ContainsIgnoringCase(word)
                       || (te.Project != null && te.Project.Name.ContainsIgnoringCase(word))
                       || (te.Project?.Client != null && te.Project.Client.Name.ContainsIgnoringCase(word)));

        private Func<IEnumerable<IDatabaseProject>, IEnumerable<IDatabaseProject>> filterProjectsByWord(string word)
            => projects =>
                projects.Where(
                    p => p.Name.ContainsIgnoringCase(word)
                      || (p.Client != null && p.Client.Name.ContainsIgnoringCase(word)));
    }
}
