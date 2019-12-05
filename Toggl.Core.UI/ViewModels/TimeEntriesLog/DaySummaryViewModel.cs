using System;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;

namespace Toggl.Core.UI.ViewModels.TimeEntriesLog
{
    public sealed class DaySummaryViewModel : MainLogSectionViewModel
    {
        public string Title { get; }

        public string TotalTrackedTime { get; }

        public DaySummaryViewModel(DateTime day, string title, string totalTrackedTime)
        {
            Title = title;
            TotalTrackedTime = totalTrackedTime;
            Identity = new DayHeaderKey(day);
        }

        public override bool Equals(MainLogItemViewModel logItem)
        {
            if (ReferenceEquals(null, logItem)) return false;
            if (ReferenceEquals(this, logItem)) return true;
            if (!(logItem is DaySummaryViewModel other)) return false;

            return Title == other.Title && TotalTrackedTime == other.TotalTrackedTime;
        }
    }
}
