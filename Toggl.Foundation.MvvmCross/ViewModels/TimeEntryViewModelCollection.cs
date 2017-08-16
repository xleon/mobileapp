using System;
using System.Collections.ObjectModel;
using System.Linq;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntryViewModelCollection : ObservableCollection<TimeEntryViewModel>
    {
        private readonly IDisposable timeDisposable;

        public DateTime Date { get; }

        public TimeSpan TotalTime { get; set; }

        public TimeEntryViewModelCollection(IGrouping<DateTime, IDatabaseTimeEntry> grouping, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(grouping, nameof(grouping));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            Date = grouping.Key;

            DateTimeOffset? runningTimeEntryStartTime = null;

            TotalTime = grouping.Aggregate(TimeSpan.Zero, (acc, timeEntry) =>
            {
                Add(new TimeEntryViewModel(timeEntry, timeService));

                if (timeEntry.Stop == null)
                {
                    runningTimeEntryStartTime = timeEntry.Start;
                    return acc;
                }

                var timeEntryDuration = timeEntry.Stop.Value - timeEntry.Start;
                return acc + timeEntryDuration;
            });

            if (runningTimeEntryStartTime == null) return;

            var timeSpanTime = TotalTime;
            timeDisposable = timeService
                .CurrentDateTimeObservable
                .Subscribe(currentTime =>
                    TotalTime = timeSpanTime + (currentTime - runningTimeEntryStartTime.Value)
                );
        }
    }
}
