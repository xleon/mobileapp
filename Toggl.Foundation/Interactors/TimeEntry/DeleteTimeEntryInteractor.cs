using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Interactors
{
    internal sealed class DeleteTimeEntryInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly long id;
        private readonly ITimeService timeService;
        private readonly IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource;

        public DeleteTimeEntryInteractor(
            ITimeService timeService,
            IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> dataSource,
            long id)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.id = id;
            this.dataSource = dataSource;
            this.timeService = timeService;
        }

        public IObservable<Unit> Execute()
            => dataSource.GetById(id)
                .Select(TimeEntry.DirtyDeleted)
                .Select(te => te.UpdatedAt(timeService.CurrentDateTime))
                .SelectMany(dataSource.Update)
                .Do(notifyTimeEntryDeleted)
                .SelectUnit();

        private void notifyTimeEntryDeleted(IThreadSafeTimeEntry timeEntry)
        {
            if (dataSource is TimeEntriesDataSource timeEntriesDataSource)
                timeEntriesDataSource.OnTimeEntrySoftDeleted(timeEntry);
        }
    }
}
