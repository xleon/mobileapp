using System;
using Toggl.Core.MvvmCross.Collections;
using Toggl.Core.MvvmCross.ViewModels.TimeEntriesLog.Identity;
using Toggl.Shared;

namespace Toggl.Core.MvvmCross.ViewModels.TimeEntriesLog
{
    public sealed class DaySummaryViewModel : IDiffable<IMainLogKey>
    {
        public string Title { get; }

        public string TotalTrackedTime { get; }

        public IMainLogKey Identity { get; }

        public DaySummaryViewModel(DateTime day, string title, string totalTrackedTime)
        {
            Title = title;
            TotalTrackedTime = totalTrackedTime;
            Identity = new DayHeaderKey(day);
        }
    }
}
