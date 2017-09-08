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
using Client = Toggl.Ultrawave.Models.Client;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class PersistClientsStateTests : PersistStateTests
    {
        public PersistClientsStateTests()
            : base(new TheStartMethod())
        {
        }

        private sealed class TheStartMethod
            : TheStartMethod<PersistClientsState, IClient, IDatabaseClient>
        {
            protected override PersistClientsState CreateState(ITogglDatabase database)
                => new PersistClientsState(database);

            protected override List<IClient> CreateEmptyList() => new List<IClient>();

            protected override List<IClient> CreateListWithOneItem(DateTimeOffset? at = null)
                => new List<IClient> { new Client { At = at ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() } };

            protected override FetchObservables CreateObservablesWhichFetchesTwice()
                => CreateFetchObservables(
                    null, new SinceParameters(null),
                    clients: Observable.Create<List<IClient>>(observer =>
                    {
                        observer.OnNext(CreateEmptyList());
                        observer.OnNext(CreateEmptyList());
                        return () => { };
                    }));

            protected override bool OtherSinceDatesDidntChange(ISinceParameters old, ISinceParameters next, DateTimeOffset at)
                => next.Workspaces == old.Workspaces
                   && next.Projects == old.Projects
                   && next.Clients == at
                   && next.Tags == old.Tags
                   && next.Tasks == old.Tasks;

            protected override FetchObservables CreateObservables(
                ISinceParameters since = null,
                List<IClient> clients = null)
            => new FetchObservables(
                since ?? new SinceParameters(null),
                Observable.Return(new List<IWorkspace>()),
                Observable.Return(clients ?? new List<IClient>()),
                Observable.Return(new List<IProject>()),
                Observable.Return(new List<ITimeEntry>()));

            protected override List<IClient> CreateComplexListWhereTheLastUpdateEntityIsDeleted(DateTimeOffset? at)
                => new List<IClient>
                {
                    new Client { At = at?.AddDays(-1) ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() },
                    new Client { At = at?.AddDays(-3) ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() },
                    new Client { At = at ?? DateTimeOffset.Now, ServerDeletedAt = at, Name = Guid.NewGuid().ToString() },
                    new Client { At = at?.AddDays(-2) ?? DateTimeOffset.Now, Name = Guid.NewGuid().ToString() }
                };

            protected override void SetupDatabaseBatchUpdateMocksToReturnDatabaseEntitiesAndFilterOutDeletedEntities(ITogglDatabase database, List<IClient> clients = null)
            {
                var foundationClients = clients?.Where(client => client.ServerDeletedAt == null).Select(Models.Client.Clean);
                database.Clients.BatchUpdate(null, null)
                    .ReturnsForAnyArgs(Observable.Return(foundationClients));
            }

            protected override void SetupDatabaseBatchUpdateToThrow(ITogglDatabase database, Func<Exception> exceptionFactory)
                => database.Clients.BatchUpdate(null, null).ReturnsForAnyArgs(_ => throw exceptionFactory());

            protected override void AssertBatchUpdateWasCalled(ITogglDatabase database, List<IClient> clients = null)
            {
                database.Clients.Received().BatchUpdate(Arg.Is<IEnumerable<(long, IDatabaseClient Client)>>(
                        list => list.Count() == clients.Count && list.Select(pair => pair.Client).All(shouldBePersistedAndIsClean(clients))),
                    Arg.Any<Func<IDatabaseClient, IDatabaseClient, ConflictResolutionMode>>());
            }

            private Func<IDatabaseClient, bool> shouldBePersistedAndIsClean(List<IClient> clients)
                => persisted => persisted.SyncStatus == SyncStatus.InSync && clients.Any(w => w.Name == persisted.Name);
        }
    }
}
