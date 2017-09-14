using Toggl.Foundation.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Tests.Sync.States
{
    internal sealed class UnsyncableTimeEntryState : BaseUnsyncableEntityState<IDatabaseTimeEntry>
    {
        public UnsyncableTimeEntryState(ITogglDatabase database) : base(database)
        {
        }

        protected override bool HasChanged(IDatabaseTimeEntry original, IDatabaseTimeEntry current)
            => original.At < current.At;

        protected override IRepository<IDatabaseTimeEntry> GetRepository(ITogglDatabase database)
            => database.TimeEntries;

        protected override IDatabaseTimeEntry CreateUnsyncableFrom(IDatabaseTimeEntry entity, string reason)
            => TimeEntry.Unsyncable(entity, reason);
    }
}
