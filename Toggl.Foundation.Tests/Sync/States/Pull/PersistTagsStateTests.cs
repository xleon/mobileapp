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
using Tag = Toggl.Ultrawave.Models.Tag;
using static Toggl.PrimeRadiant.ConflictResolutionMode;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class PersistTagsStateTests : PersistStateTests
    {
        public PersistTagsStateTests()
            : base(new TheStartMethod())
        {
        }

        private sealed class TheStartMethod
            : TheStartMethod<PersistTagsState, ITag, IDatabaseTag>
        {
            protected override PersistTagsState CreateState(ITogglDatabase database)
                => new PersistTagsState(database);

            protected override List<ITag> CreateEmptyList() => new List<ITag>();

            protected override List<ITag> CreateListWithOneItem(DateTimeOffset? at = null)
                => new List<ITag> { new Tag { At = at ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() } };

            protected override FetchObservables CreateObservablesWhichFetchesTwice()
                => CreateFetchObservables(
                    null, new SinceParameters(null),
                    tags: Observable.Create<List<ITag>>(observer =>
                    {
                        observer.OnNext(CreateEmptyList());
                        observer.OnNext(CreateEmptyList());
                        return () => { };
                    }));

            protected override bool OtherSinceDatesDidntChange(ISinceParameters old, ISinceParameters next, DateTimeOffset at)
                => next.Workspaces == old.Workspaces
                   && next.Projects == old.Projects
                   && next.Clients == old.Clients
                   && next.Tags == at
                   && next.Tasks == old.Tasks
                   && next.TimeEntries == old.TimeEntries;

            protected override FetchObservables CreateObservables(
                ISinceParameters since = null,
                List<ITag> tags = null)
            => new FetchObservables(
                    since ?? new SinceParameters(null),
                    Observable.Return(new List<IWorkspace>()),
                    Observable.Return(new List<IClient>()),
                    Observable.Return(new List<IProject>()),
                    Observable.Return(new List<ITimeEntry>()),
                    Observable.Return(tags));

            protected override List<ITag> CreateComplexListWhereTheLastUpdateEntityIsDeleted(DateTimeOffset? at)
                => createComplexList(at ?? DateTimeOffset.Now);

            private List<ITag> createComplexList(DateTimeOffset at)
                => new List<ITag>
                {
                    new Tag { At = at.AddDays(-1), Name = Guid.NewGuid().ToString() },
                    new Tag { At = at.AddDays(-3), Name = Guid.NewGuid().ToString() },
                    new Tag { At = at, Name = Guid.NewGuid().ToString() },
                    new Tag { At = at.AddDays(-2), Name = Guid.NewGuid().ToString() }
                };

            protected override void SetupDatabaseBatchUpdateMocksToReturnUpdatedDatabaseEntitiesAndSimulateDeletionOfEntities(ITogglDatabase database, List<ITag> tags = null)
            {
                var foundationTags = tags?.Select(tag => (Update, (IDatabaseTag)Models.Tag.Clean(tag)));
                database.Tags.BatchUpdate(null, null)
                    .ReturnsForAnyArgs(Observable.Return(foundationTags));
            }

            protected override void SetupDatabaseBatchUpdateToThrow(ITogglDatabase database, Func<Exception> exceptionFactory)
                => database.Tags.BatchUpdate(null, null).ReturnsForAnyArgs(_ => throw exceptionFactory());

            protected override void AssertBatchUpdateWasCalled(ITogglDatabase database, List<ITag> tags = null)
            {
                database.Tags.Received().BatchUpdate(Arg.Is<IEnumerable<(long, IDatabaseTag Tag)>>(
                        list => list.Count() == tags.Count && list.Select(pair => pair.Tag).All(shouldBePersistedAndIsClean(tags))),
                    Arg.Any<Func<IDatabaseTag, IDatabaseTag, ConflictResolutionMode>>());
            }

            private Func<IDatabaseTag, bool> shouldBePersistedAndIsClean(List<ITag> tags)
                => persisted => persisted.SyncStatus == SyncStatus.InSync && tags.Any(te => te.Name == persisted.Name);
        }
    }
}
