using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.Collections
{
    public sealed class TimeEntryViewModelCollection : MvxObservableCollection<TimeEntryViewModel>
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
