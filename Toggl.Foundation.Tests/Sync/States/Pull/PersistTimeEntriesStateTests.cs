using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NSubstitute;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using TimeEntry = Toggl.Ultrawave.Models.TimeEntry;
using static Toggl.PrimeRadiant.ConflictResolutionMode;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class PersistTimeEntriesStateTests : PersistStateTests
    {
        public PersistTimeEntriesStateTests()
            : base(new TheStartMethod())
        {
        }

        private sealed class TheStartMethod
            : TheStartMethod<PersistTimeEntriesState, ITimeEntry, IDatabaseTimeEntry>
        {
            protected override PersistTimeEntriesState CreateState(ITogglDatabase database)
                => new PersistTimeEntriesState(database);

            protected override List<ITimeEntry> CreateEmptyList() => new List<ITimeEntry>();

            protected override List<ITimeEntry> CreateListWithOneItem(DateTimeOffset? at = null)
                => new List<ITimeEntry> { new TimeEntry { At = at ?? DateTimeOffset.Now, Description = Guid.NewGuid().ToString() } };

            protected override FetchObservables CreateObservablesWhichFetchesTwice()
                => CreateFetchObservables(
                    null, new SinceParameters(null),
                    timeEntries: Observable.Create<List<ITimeEntry>>(observer =>
                    {
                        observer.OnNext(CreateEmptyList());
                        observer.OnNext(CreateEmptyList());
                        return () => { };
                    }));

            protected override bool OtherSinceDatesDidntChange(ISinceParameters old, ISinceParameters next, DateTimeOffset at)
                => next.Workspaces == old.Workspaces
                   && next.Projects == old.Projects
                   && next.Clients == old.Clients
                   && next.Tags == old.Tags
                   && next.Tasks == old.Tasks
                   && next.TimeEntries == at;

            protected override FetchObservables CreateObservables(
                ISinceParameters since = null,
                List<ITimeEntry> timeEntries = null)
            => new FetchObservables(
                    since ?? new SinceParameters(null),
                    Observable.Return(new List<IWorkspace>()),
                    Observable.Return(new List<IClient>()),
                    Observable.Return(new List<IProject>()),
                    Observable.Return(timeEntries),
                    Observable.Return(new List<ITag>()));

            protected override List<ITimeEntry> CreateComplexListWhereTheLastUpdateEntityIsDeleted(DateTimeOffset? at)
                => createComplexList(at ?? DateTimeOffset.Now);

            private List<ITimeEntry> createComplexList(DateTimeOffset at)
                => new List<ITimeEntry>
                {
                    new TimeEntry { At = at.AddDays(-1), Description = Guid.NewGuid().ToString() },
                    new TimeEntry { At = at.AddDays(-3), Description = Guid.NewGuid().ToString() },
                    new TimeEntry { At = at, ServerDeletedAt = at, Description = Guid.NewGuid().ToString() },
                    new TimeEntry { At = at.AddDays(-2), Description = Guid.NewGuid().ToString() }
                };

            protected override void SetupDatabaseBatchUpdateMocksToReturnUpdatedDatabaseEntitiesAndSimulateDeletionOfEntities(ITogglDatabase database, List<ITimeEntry> timeEntries = null)
            {
                var foundationTimeEntries = timeEntries?.Select(timeEntry => timeEntry.ServerDeletedAt.HasValue
                        ? (Delete, null)
                        : (Update, (IDatabaseTimeEntry)Models.TimeEntry.Clean(timeEntry)));
                database.TimeEntries.BatchUpdate(null, null)
                    .ReturnsForAnyArgs(Observable.Return(foundationTimeEntries));
            }

            protected override void SetupDatabaseBatchUpdateToThrow(ITogglDatabase database, Func<Exception> exceptionFactory)
                => database.TimeEntries.BatchUpdate(null, null).ReturnsForAnyArgs(_ => throw exceptionFactory());

            protected override void AssertBatchUpdateWasCalled(ITogglDatabase database, List<ITimeEntry> timeEntries = null)
            {
                database.TimeEntries.Received().BatchUpdate(Arg.Is<IEnumerable<(long, IDatabaseTimeEntry TimeEntry)>>(
                        list => list.Count() == timeEntries.Count && list.Select(pair => pair.TimeEntry).All(shouldBePersistedAndIsClean(timeEntries))),
                    Arg.Any<Func<IDatabaseTimeEntry, IDatabaseTimeEntry, ConflictResolutionMode>>());
            }

            private Func<IDatabaseTimeEntry, bool> shouldBePersistedAndIsClean(List<ITimeEntry> timeEntries)
                => persisted => persisted.SyncStatus == SyncStatus.InSync && timeEntries.Any(te => te.Description == persisted.Description);
        }
    }
}
