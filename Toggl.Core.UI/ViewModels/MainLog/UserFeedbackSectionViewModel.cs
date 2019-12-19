using Toggl.Core.UI.ViewModels.MainLog.Identity;

namespace Toggl.Core.UI.ViewModels.MainLog
{
    public class UserFeedbackSectionViewModel : MainLogSectionViewModel
    {
        public UserFeedbackSectionViewModel()
        {
            Identity = new UserFeedbackSectionKey();
        }

        public override bool Equals(MainLogItemViewModel logItem)
        {
            if (ReferenceEquals(null, logItem)) return false;
            if (ReferenceEquals(this, logItem)) return true;
            return logItem is UserFeedbackSectionViewModel;
        }
    }
}
