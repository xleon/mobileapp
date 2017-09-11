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
using Project = Toggl.Ultrawave.Models.Project;
using static Toggl.PrimeRadiant.ConflictResolutionMode;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class PersistProjectsStateTests : PersistStateTests
    {
        public PersistProjectsStateTests()
            : base(new TheStartMethod())
        {
        }

        private sealed class TheStartMethod
            : TheStartMethod<PersistProjectsState, IProject, IDatabaseProject>
        {
            protected override PersistProjectsState CreateState(ITogglDatabase database)
                => new PersistProjectsState(database);

            protected override List<IProject> CreateEmptyList() => new List<IProject>();

            protected override List<IProject> CreateListWithOneItem(DateTimeOffset? at = null)
                => new List<IProject> { new Project { At = at ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() } };

            protected override FetchObservables CreateObservablesWhichFetchesTwice()
                => CreateFetchObservables(
                    null, new SinceParameters(null),
                    projects: Observable.Create<List<IProject>>(observer =>
                    {
                        observer.OnNext(CreateEmptyList());
                        observer.OnNext(CreateEmptyList());
                        return () => { };
                    }));

            protected override bool OtherSinceDatesDidntChange(ISinceParameters old, ISinceParameters next, DateTimeOffset at)
                => next.Workspaces == old.Workspaces
                   && next.Clients == old.Clients
                   && next.Projects == at
                   && next.Tags == old.Tags
                   && next.Tasks == old.Tasks
                   && next.TimeEntries == old.TimeEntries;

            protected override FetchObservables CreateObservables(
                ISinceParameters since = null,
                List<IProject> projects = null)
            => new FetchObservables(
                since ?? new SinceParameters(null),
                Observable.Return(new List<IWorkspace>()),
                Observable.Return(new List<IClient>()),
                Observable.Return(projects ?? new List<IProject>()),
                Observable.Return(new List<ITimeEntry>()),
                Observable.Return(new List<ITag>()));

            protected override List<IProject> CreateComplexListWhereTheLastUpdateEntityIsDeleted(DateTimeOffset? at)
                => new List<IProject>
                {
                    new Project { At = at?.AddDays(-1) ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() },
                    new Project { At = at?.AddDays(-3) ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() },
                    new Project { At = at ?? DateTimeOffset.Now, ServerDeletedAt = at, Name = Guid.NewGuid().ToString() },
                    new Project { At = at?.AddDays(-2) ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() }
                };

            protected override void SetupDatabaseBatchUpdateMocksToReturnUpdatedDatabaseEntitiesAndSimulateDeletionOfEntities(ITogglDatabase database, List<IProject> workspaces = null)
            {
                var foundationWorkspaces = workspaces?.Select(project => project.ServerDeletedAt.HasValue
                    ? (Delete, null)
                    : (Update, (IDatabaseProject)Models.Project.Clean(project)));
                database.Projects.BatchUpdate(null, null)
                    .ReturnsForAnyArgs(Observable.Return(foundationWorkspaces));
            }

            protected override void SetupDatabaseBatchUpdateToThrow(ITogglDatabase database, Func<Exception> exceptionFactory)
                => database.Projects.BatchUpdate(null, null).ReturnsForAnyArgs(_ => throw exceptionFactory());

            protected override void AssertBatchUpdateWasCalled(ITogglDatabase database, List<IProject> projects = null)
            {
                database.Projects.Received().BatchUpdate(Arg.Is<IEnumerable<(long, IDatabaseProject Project)>>(
                        list => list.Count() == projects.Count && list.Select(pair => pair.Project).All(shouldBePersistedAndIsClean(projects))),
                    Arg.Any<Func<IDatabaseProject, IDatabaseProject, ConflictResolutionMode>>());
            }

            private Func<IDatabaseProject, bool> shouldBePersistedAndIsClean(List<IProject> projects)
                => persisted => persisted.SyncStatus == SyncStatus.InSync && projects.Any(w => w.Name == persisted.Name);
        }
    }
}
