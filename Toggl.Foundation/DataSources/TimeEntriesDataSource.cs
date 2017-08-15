using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive.Subjects;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class TimeEntriesDataSource : ITimeEntriesSource
    {
        private readonly IIdProvider idProvider;
        private readonly IRepository<IDatabaseTimeEntry> repository;
        private readonly BehaviorSubject<IDatabaseTimeEntry> currentTimeEntrySubject = new BehaviorSubject<IDatabaseTimeEntry>(null);

        public IObservable<IDatabaseTimeEntry> CurrentlyRunningTimeEntry { get; }

        public TimeEntriesDataSource(IIdProvider idProvider, IRepository<IDatabaseTimeEntry> repository)
        {
            Ensure.Argument.IsNotNull(repository, nameof(repository));

            this.repository = repository;
            this.idProvider = idProvider;

            CurrentlyRunningTimeEntry = currentTimeEntrySubject.AsObservable().DistinctUntilChanged();

            repository
                .GetAll(te => te.Stop == null)
                .Subscribe(onRunningTimeEntry);
        }

        public IObservable<IEnumerable<IDatabaseTimeEntry>> GetAll()
            => repository.GetAll(te => !te.IsDeleted);

        public IObservable<IDatabaseTimeEntry> GetById(long id)
            => repository.GetById(id);

        public IObservable<Unit> Delete(long id)
            => repository.GetById(id)
                         .Select(TimeEntry.DirtyDeleted)
                         .SelectMany(repository.Update)
                         .IgnoreElements()
                         .Cast<Unit>();

        public IObservable<IDatabaseTimeEntry> Start(DateTimeOffset startTime, string description, bool billable)
            => idProvider.GetNextIdentifier()
                  .Apply(TimeEntry.Builder.Create)
                  .SetStart(startTime)
                  .SetDescription(description)
                  .SetBillable(billable)
                  .SetIsDirty(true)
                  .Build()
                  .Apply(repository.Create)
                  .Do(safeSetCurrentlyRunningTimeEntry);

        public IObservable<IDatabaseTimeEntry> Stop(DateTimeOffset stopTime)
            => repository
                    .GetAll(te => te.Stop == null)
                    .SelectMany(timeEntries =>
                        timeEntries.Single()
                            .With(stopTime)
                            .Apply(repository.Update)
                            .Do(_ => safeSetCurrentlyRunningTimeEntry(null)));
        
        private void onRunningTimeEntry(IEnumerable<IDatabaseTimeEntry> timeEntries)
            => safeSetCurrentlyRunningTimeEntry(timeEntries.SingleOrDefault());
            
        private void safeSetCurrentlyRunningTimeEntry(IDatabaseTimeEntry timeEntry)
        {
            var next = timeEntry == null ? null : TimeEntry.Clean(timeEntry);
            currentTimeEntrySubject.OnNext(next);
        }
    }
}
