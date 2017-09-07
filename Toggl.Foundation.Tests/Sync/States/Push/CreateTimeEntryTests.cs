using System;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class CreateTimeEntryTests : BaseCreateEntityStateTests
    {
        public CreateTimeEntryTests()
            : base(new TheStartMethod())
        {
        }

        private class TheStartMethod : TheStartMethod<IDatabaseTimeEntry, ITimeEntry>
        {
            protected override IDatabaseTimeEntry CreateDirtyEntityWithNegativeId()
                => TimeEntry.Dirty(new Ultrawave.Models.TimeEntry { Id = -123, Description = Guid.NewGuid().ToString() });

            protected override IDatabaseTimeEntry CreateCleanWithPositiveIdFrom(IDatabaseTimeEntry entity)
                => TimeEntry.Clean(new Ultrawave.Models.TimeEntry { Id = 456, Description = entity.Description });

            protected override IDatabaseTimeEntry CreateCleanEntityFrom(IDatabaseTimeEntry entity)
                => TimeEntry.Clean(entity);

            protected override BaseCreateEntityState<IDatabaseTimeEntry> CreateState(ITogglApi api, ITogglDatabase database)
                => new CreateTimeEntryState(api, database);

            protected override IRepository<IDatabaseTimeEntry> GetRepository(ITogglDatabase database)
                => database.TimeEntries;

            protected override Func<IDatabaseTimeEntry, IObservable<ITimeEntry>> GetCreateFunction(ITogglApi api)
                => api.TimeEntries.Create;

            protected override bool EntitiesHaveSameImportantProperties(IDatabaseTimeEntry a, IDatabaseTimeEntry b)
                => a.Description == b.Description;
        }
    }
}
