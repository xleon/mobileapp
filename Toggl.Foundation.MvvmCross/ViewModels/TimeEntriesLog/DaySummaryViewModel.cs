namespace Toggl.Foundation.MvvmCross.ViewModels.TimeEntriesLog
{
    public sealed class DaySummaryViewModel
    {
        public string Title { get; }

        public string TotalTrackedTime { get; }

        public DaySummaryViewModel(string title, string totalTrackedTime)
        {
            Title = title;
            TotalTrackedTime = totalTrackedTime;
        }
    }
}
