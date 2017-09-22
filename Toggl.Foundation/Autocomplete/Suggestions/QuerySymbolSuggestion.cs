using System;

namespace Toggl.Foundation.Autocomplete.Suggestions
{
    public sealed class QuerySymbolSuggestion : AutocompleteSuggestion
    {
        internal static QuerySymbolSuggestion[] Suggestions { get; } =
        {
            new QuerySymbolSuggestion(QuerySymbols.ProjectsString, nameof(QuerySymbols.Projects)),
            new QuerySymbolSuggestion(QuerySymbols.TagsString, nameof(QuerySymbols.Tags))
        };

        public string Symbol { get; }

        public string Description { get; }

        private QuerySymbolSuggestion(string symbol, string suggestionName)
        {
            Symbol = symbol;
            Description = $"Search {suggestionName}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Symbol.GetHashCode() * 397) ^ Description.GetHashCode();
            }
        }
    }
}
