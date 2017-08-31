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
using Workspace = Toggl.Ultrawave.Models.Workspace;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class PersistWorkspacesStateTests : PersistStateTests
    {
        public PersistWorkspacesStateTests()
            : base(new TheStartMethod())
        {
        }

        private sealed class TheStartMethod
            : TheStartMethod<PersistWorkspacesState, IWorkspace, IDatabaseWorkspace>
        {
            protected override PersistWorkspacesState CreateState(ITogglDatabase database)
                => new PersistWorkspacesState(database);

            protected override List<IWorkspace> CreateEmptyList() => new List<IWorkspace>();

            protected override List<IWorkspace> CreateListWithOneItem(DateTimeOffset? at = null)
                => new List<IWorkspace> { new Workspace { At = at, Name = Guid.NewGuid().ToString() } };

            protected override FetchObservables CreateObservablesWhichFetchesTwice()
                => CreateFetchObservables(
                    null, new SinceParameters(null),
                    Observable.Create<List<IWorkspace>>(observer =>
                    {
                        observer.OnNext(CreateEmptyList());
                        observer.OnNext(CreateEmptyList());
                        return () => { };
                    }));

            protected override bool OtherSinceDatesDidntChange(ISinceParameters old, ISinceParameters next, DateTimeOffset at)
                => next.Workspaces == at
                   && next.Projects == old.Projects
                   && next.Clients == old.Clients
                   && next.Tags == old.Tags
                   && next.Tasks == old.Tasks;

            protected override FetchObservables CreateObservables(
                ISinceParameters since = null,
                List<IWorkspace> workspaces = null)
            => new FetchObservables(
                since ?? new SinceParameters(null),
                Observable.Return(workspaces ?? new List<IWorkspace>()),
                Observable.Return(new List<IClient>()),
                Observable.Return(new List<IProject>()),
                Observable.Return(new List<ITimeEntry>()));

            protected override List<IWorkspace> CreateComplexListWhereTheLastUpdateEntityIsDeleted(DateTimeOffset? at)
                => new List<IWorkspace>
                {
                    new Workspace { At = at?.AddDays(-1), Name = Guid.NewGuid().ToString() },
                    new Workspace { At = at?.AddDays(-3), Name = Guid.NewGuid().ToString() },
                    new Workspace { At = at, ServerDeletedAt = at, Name = Guid.NewGuid().ToString() },
                    new Workspace { At = at?.AddDays(-2), Name = Guid.NewGuid().ToString() }
                };

            protected override void SetupDatabaseBatchUpdateMocksToReturnDatabaseEntitiesAndFilterOutDeletedEntities(ITogglDatabase database, List<IWorkspace> workspaces = null)
            {
                var foundationWorkspaces = workspaces?.Where(workspace => workspace.ServerDeletedAt == null).Select(Models.Workspace.Clean);
                database.Workspaces.BatchUpdate(null, null)
                    .ReturnsForAnyArgs(Observable.Return(foundationWorkspaces));
            }

            protected override void SetupDatabaseBatchUpdateToThrow(ITogglDatabase database, Func<Exception> exceptionFactory)
                => database.Workspaces.BatchUpdate(null, null).ReturnsForAnyArgs(_ => throw exceptionFactory());

            protected override void AssertBatchUpdateWasCalled(ITogglDatabase database, List<IWorkspace> workspaces = null)
            {
                database.Workspaces.Received().BatchUpdate(Arg.Is<IEnumerable<IDatabaseWorkspace>>(
                        list => list.Count() == workspaces.Count && list.All(shouldBePersistedAndIsClean(workspaces))),
                    Arg.Any<Func<IDatabaseWorkspace, IDatabaseWorkspace, ConflictResolutionMode>>());
            }

            private Func<IDatabaseWorkspace, bool> shouldBePersistedAndIsClean(List<IWorkspace> workspaces)
                => persisted => persisted.IsDirty == false && workspaces.Any(w => w.Name == persisted.Name);
        }
    }
}
