using Toggl.Core.Suggestions;
using Toggl.Core.UI.ViewModels.MainLog.Identity;

namespace Toggl.Core.UI.ViewModels.MainLog
{
    public sealed class SuggestionLogItemViewModel : MainLogItemViewModel
    {
        public Suggestion Suggestion { get; }

        public SuggestionLogItemViewModel(long id, Suggestion suggestion)
        {
            Identity = new SuggestionKey(id);
            Suggestion = suggestion;
        }

        public override bool Equals(MainLogItemViewModel logItem)
        {
            if (ReferenceEquals(null, logItem)) return false;
            if (ReferenceEquals(this, logItem)) return true;
            if (!(logItem is SuggestionLogItemViewModel other)) return false;

            return Suggestion.Equals(other.Suggestion);
        }
    }
}
