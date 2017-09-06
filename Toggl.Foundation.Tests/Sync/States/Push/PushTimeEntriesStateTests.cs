using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NSubstitute;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync.States;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Tests.Sync.States.Push
{
    public class PushTimeEntriesStateTests : BasePushStateTests
    {
        public PushTimeEntriesStateTests()
            : base(new TheStartMethod())
        {
        }

        private class TheStartMethod : TheStartMethod<IDatabaseTimeEntry>
        {
            protected override BasePushState<IDatabaseTimeEntry> CreateState(ITogglDatabase database)
                => new PushTimeEntriesState(database);

            protected override IDatabaseTimeEntry CreateUnsyncedEntity(DateTimeOffset lastUpdate = default(DateTimeOffset))
                => TimeEntry.Dirty(new Ultrawave.Models.TimeEntry { At = lastUpdate });

            protected override void SetupRepositoryToReturn(ITogglDatabase database, IDatabaseTimeEntry[] entities)
            {
                database.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(Observable.Return(entities));
            }

            protected override void SetupRepositoryToThrow(ITogglDatabase database)
            {
                database.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(_ => { throw new TestException(); });
            }
        }
    }
}
