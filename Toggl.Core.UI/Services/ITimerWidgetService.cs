using System.Collections.Immutable;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;

namespace Toggl.Core.UI.Services
{
    public interface ITimerWidgetService
    {
        void Start();
        void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry);
        void OnSuggestionsUpdated(IImmutableList<Suggestion> suggestions);
    }
}
