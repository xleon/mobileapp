using System;

namespace Toggl.Foundation.Suggestions
{
    public interface ISuggestionProvider
    {
        IObservable<Suggestion> GetSuggestions();
    }
}
