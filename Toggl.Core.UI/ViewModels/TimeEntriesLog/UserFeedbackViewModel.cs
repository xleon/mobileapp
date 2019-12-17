using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.TimeEntriesLog
{
    public class UserFeedbackViewModel : MainLogSectionViewModel
    {
        public RatingViewModel RatingViewModel { get; }

        public UserFeedbackViewModel(RatingViewModel ratingViewModel)
        {
            RatingViewModel = ratingViewModel;
            Identity = new UserFeedbackKey();
        }

        public override bool Equals(MainLogItemViewModel logItem)
        {
            if (ReferenceEquals(null, logItem)) return false;
            if (ReferenceEquals(this, logItem)) return true;
            if (!(logItem is UserFeedbackViewModel other)) return false;
            return RatingViewModel.Equals(other.RatingViewModel);
        }
    }
}
