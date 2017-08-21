using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntryViewModelCollection : ObservableCollection<TimeEntryViewModel>
    {
        public DateTimeOffset Date { get; }

        public TimeSpan TotalTime { get; }

        public TimeEntryViewModelCollection(DateTime date, IEnumerable<TimeEntryViewModel> items)
        {
            Ensure.Argument.IsNotNull(items, nameof(items));

            this.AddRange(items);

            Date = new DateTimeOffset(date);
            TotalTime = items.Aggregate(TimeSpan.Zero, (acc, vm) => acc + vm.Duration);
        }
    }
}
