using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.States
{
    class PersistTimeEntriesState : BasePersistState<ITimeEntry, IDatabaseTimeEntry>
    {
        public PersistTimeEntriesState(ITogglDatabase database)
            : base(database)
        {
        }

        protected override IObservable<IEnumerable<ITimeEntry>> FetchObservable(FetchObservables fetch)
            => fetch.TimeEntries;

        protected override IDatabaseTimeEntry ConvertToDatabaseEntity(ITimeEntry entity)
            => TimeEntry.Clean(entity);

        protected override IObservable<IEnumerable<IDatabaseTimeEntry>> BatchUpdate(ITogglDatabase database, IEnumerable<(long, IDatabaseTimeEntry)> entities)
            => database.TimeEntries.BatchUpdate(entities, Resolver.ForTimeEntries().Resolve);

        protected override DateTimeOffset? LastUpdated(ISinceParameters old, IEnumerable<IDatabaseTimeEntry> entities)
            => entities.Select(p => p?.At).Where(d => d.HasValue).DefaultIfEmpty(old.TimeEntries).Max();

        protected override ISinceParameters UpdateSinceParameters(ISinceParameters old, DateTimeOffset? lastUpdated)
            => new SinceParameters(old, timeEntries: lastUpdated);
    }
}
