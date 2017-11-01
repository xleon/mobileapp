using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Toggl.Foundation.Models;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.ConflictResolution
{
    internal sealed class TimeEntryRivalsResolver : IRivalsResolver<IDatabaseTimeEntry>
    {
        private ITimeService timeService;

        public TimeEntryRivalsResolver(ITimeService timeService)
        {
            this.timeService = timeService;
        }

        public bool CanHaveRival(IDatabaseTimeEntry entity) => entity.IsRunning();

        public Expression<Func<IDatabaseTimeEntry, bool>> AreRivals(IDatabaseTimeEntry entity)
        {
            if (!CanHaveRival(entity))
                throw new InvalidOperationException("The entity cannot have any rivals.");

            return potentialRival => potentialRival.IsRunning() && potentialRival.Id != entity.Id;
        }

        public (IDatabaseTimeEntry FixedEntity, IDatabaseTimeEntry FixedRival) FixRivals(IDatabaseTimeEntry entity, IDatabaseTimeEntry rival, IQueryable<IDatabaseTimeEntry> allTimeEntries)
            => rival.At < entity.At ? (entity, stop(rival, allTimeEntries)) : (stop(entity, allTimeEntries), rival);

        private IDatabaseTimeEntry stop(IDatabaseTimeEntry toBeStopped, IQueryable<IDatabaseTimeEntry> allTimeEntries)
        {
            var stop = ((IEnumerable<IDatabaseTimeEntry>)allTimeEntries.Where(other => other.Start > toBeStopped.Start))
                .Select(te => te.Start)
                .Where(start => start != default(DateTimeOffset))
                .DefaultIfEmpty(timeService.CurrentDateTime)
                .Min();
            long duration = (long)(stop - toBeStopped.Start).TotalSeconds; // truncates towards zero (floor)
            return new TimeEntry(toBeStopped, duration);
        }
    }
}
