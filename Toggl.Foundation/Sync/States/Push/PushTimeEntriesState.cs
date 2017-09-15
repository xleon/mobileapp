using System;
using Toggl.Foundation.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.States
{
    internal sealed class PushTimeEntriesState : BasePushState<IDatabaseTimeEntry>
    {
        public PushTimeEntriesState(ITogglDatabase database)
            : base(database)
        {
        }

        protected override IRepository<IDatabaseTimeEntry> GetRepository(ITogglDatabase database)
            => database.TimeEntries;

        protected override DateTimeOffset LastChange(IDatabaseTimeEntry entity)
            => entity.At;

        protected override IDatabaseTimeEntry CopyFrom(IDatabaseTimeEntry entity)
            => TimeEntry.From(entity);
    }
}
