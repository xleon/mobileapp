using System;
using System.Diagnostics.Tracing;
using Toggl.Foundation.Autocomplete.Suggestions;

namespace Toggl.Foundation.Autocomplete
{
    public struct QueryInfo
    {
        public string Text { get; }

        public AutocompleteSuggestionType SuggestionType { get; }

        private const int minimumQueryLength = 2;

        private static QueryInfo emptyQueryInfo
            => new QueryInfo(String.Empty, AutocompleteSuggestionType.None);

        public QueryInfo(string text, AutocompleteSuggestionType suggestionType)
        {
            Text = text;
            SuggestionType = suggestionType;
        }

        public static QueryInfo ParseFieldInfo(TextFieldInfo info)
        {
            if (string.IsNullOrEmpty(info.Text))
                return emptyQueryInfo;

            return searchByQuerySymbols(info)
                ?? getDefaultQueryInfo(info.Text);
        }

        private static QueryInfo? searchByQuerySymbols(TextFieldInfo info)
        {
            QueryInfo? query = null;
            var stringToSearch = info.Text.Substring(0, info.DescriptionCursorPosition);

            int indexOfQuerySymbol = stringToSearch.LastIndexOfAny(getQuerySymbols(info));
            if (indexOfQuerySymbol >= 0)
            {
                var startingIndex = indexOfQuerySymbol + 1;
                var stringLength = info.Text.Length - indexOfQuerySymbol - 1;
                var type = getSuggestionType(stringToSearch[indexOfQuerySymbol]);
                var text = info.Text.Substring(startingIndex, stringLength);

                query = new QueryInfo(text, type);
            }

            return query;
        }

        private static QueryInfo getDefaultQueryInfo(string text)
            => text.Length < minimumQueryLength
                ? emptyQueryInfo
                : new QueryInfo(text, AutocompleteSuggestionType.TimeEntries);

        private static char[] getQuerySymbols(TextFieldInfo info)
            => info.ProjectId.HasValue
                ? QuerySymbols.ProjectSelected
                : QuerySymbols.All;

        private static AutocompleteSuggestionType getSuggestionType(char symbol)
            => symbol == QuerySymbols.Projects
                ? AutocompleteSuggestionType.Projects
                : AutocompleteSuggestionType.Tags;
    }
}
