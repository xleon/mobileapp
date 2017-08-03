using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive.Subjects;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class TimeEntriesDataSource : ITimeEntriesSource
    {
        private readonly IRepository<IDatabaseTimeEntry> repository;
        private readonly BehaviorSubject<ITimeEntry> currentTimeEntrySubject = new BehaviorSubject<ITimeEntry>(null);

        public IObservable<ITimeEntry> CurrentlyRunningTimeEntry { get; }

        public TimeEntriesDataSource(IRepository<IDatabaseTimeEntry> repository)
        {
            Ensure.Argument.IsNotNull(repository, nameof(repository));

            this.repository = repository;

            CurrentlyRunningTimeEntry = currentTimeEntrySubject.AsObservable().DistinctUntilChanged();

            repository
                .GetAll(runningTimeEntries)
                .Subscribe(onRunningTimeEntry);
        }

        public IObservable<IEnumerable<ITimeEntry>> GetAll()
            => repository.GetAll(te => !te.IsDeleted);

        public IObservable<Unit> Delete(int id)
            => repository.GetById(id)
                         .Select(TimeEntry.DirtyDeleted)
                         .SelectMany(repository.Update)
                         .IgnoreElements()
                         .Cast<Unit>();

        public IObservable<ITimeEntry> Start(DateTimeOffset startTime, string description, bool billable)
            => TimeEntry.Builder.Create()
                .SetStart(startTime)
                .SetDescription(description)
                .SetBillable(billable)
                .SetIsDirty(true)
                .Build()
                .Apply(repository.Create)
                .Do(currentTimeEntrySubject.OnNext);

        private bool runningTimeEntries(ITimeEntry timeEntry) => timeEntry.Stop == null;

        private void onRunningTimeEntry(IEnumerable<ITimeEntry> timeEntries)
            => currentTimeEntrySubject.OnNext(timeEntries.SingleOrDefault());

    }
}
