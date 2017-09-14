using System;
using Toggl.Foundation.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Tests.Sync.States.Push
{
    public sealed class UnsyncableTimeEntryStateTests : BaseUnsyncableEntityStateTests
    {
        public UnsyncableTimeEntryStateTests()
            : base(new TheStartMethod())
        {
        }

        private sealed class TheStartMethod : TheStartMethod<IDatabaseTimeEntry>
        {
            protected override BaseUnsyncableEntityState<IDatabaseTimeEntry> CreateState(ITogglDatabase database)
                => new UnsyncableTimeEntryState(database);

            protected override IDatabaseTimeEntry CreateDirtyEntity()
                => TimeEntry.Dirty(new Ultrawave.Models.TimeEntry { Description = Guid.NewGuid().ToString() });

            protected override IRepository<IDatabaseTimeEntry> GetRepository(ITogglDatabase database)
                => database.TimeEntries;
        }
    }
}
