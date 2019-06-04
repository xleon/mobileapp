using System;
using System.Collections.Generic;
using Toggl.Core.Interactors.Suggestions;
using Toggl.Core.Suggestions;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<IEnumerable<Suggestion>>> GetSuggestions(int count)
            => new GetSuggestionsInteractor(count, stopwatchProvider, dataSource, timeService, calendarService, GetDefaultWorkspace());
    }
}
