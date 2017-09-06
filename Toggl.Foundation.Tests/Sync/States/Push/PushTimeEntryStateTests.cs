using Toggl.Foundation.Models;
using Toggl.Foundation.Sync.States;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class PushTimeEntryStateTests : BasePushEntityStateTests
    {
        public PushTimeEntryStateTests()
            : base(new TheStartMethod())
        {
        }

        private class TheStartMethod : TheStartMethod<IDatabaseTimeEntry>
        {
            protected override BasePushEntityState<IDatabaseTimeEntry> CreateState()
                => new PushTimeEntryState();

            protected override IDatabaseTimeEntry CreatePublishedDeletedEntity()
                => TimeEntry.DirtyDeleted(new Ultrawave.Models.TimeEntry { Id = 123 });

            protected override IDatabaseTimeEntry CreateUnpublishedDeletedEntity()
                => TimeEntry.DirtyDeleted(new Ultrawave.Models.TimeEntry { Id = -123 });

            protected override IDatabaseTimeEntry CreatePublishedNotDeletedEntity()
                => TimeEntry.Dirty(new Ultrawave.Models.TimeEntry { Id = 123 });

            protected override IDatabaseTimeEntry CreateUnpublishedNotDeletedEntity()
                => TimeEntry.Dirty(new Ultrawave.Models.TimeEntry { Id = -123 });
        }
    }
}
