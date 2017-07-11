using System;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.Suggestions
{
    public interface ISuggestionProvider
    {
        IObservable<ITimeEntry> GetSuggestion();
    }
}
