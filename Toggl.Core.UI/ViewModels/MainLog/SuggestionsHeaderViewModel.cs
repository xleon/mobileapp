using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.MainLog.Identity;

namespace Toggl.Core.UI.ViewModels.MainLog
{
    public class SuggestionsHeaderViewModel : MainLogSectionViewModel
    {
        public string Title { get; }

        public SuggestionsHeaderViewModel(string title)
        {
            Title = title;
            Identity = new SuggestionsSectionKey();
        }

        public override bool Equals(MainLogItemViewModel logItem)
        {
            if (ReferenceEquals(null, logItem)) return false;
            if (ReferenceEquals(this, logItem)) return true;
            if (!(logItem is SuggestionsHeaderViewModel other)) return false;

            return Title == other.Title;
        }
    }
}
