using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.States
{
    class PersistClientsState : BasePersistState<IClient, IDatabaseClient>
    {
        public PersistClientsState(ITogglDatabase database)
            : base(database)
        {
        }

        protected override IObservable<IEnumerable<IClient>> FetchObservable(FetchObservables fetch)
            => fetch.Clients;

        protected override IDatabaseClient ConvertToDatabaseEntity(IClient entity)
            => Client.Clean(entity);

        protected override IObservable<IEnumerable<IDatabaseClient>> BatchUpdate(ITogglDatabase database, IEnumerable<IDatabaseClient> entities)
            => database.Clients.BatchUpdate(entities, Resolver.ForClients().Resolve);

        protected override DateTimeOffset? LastUpdated(ISinceParameters old, IEnumerable<IDatabaseClient> entities)
            => entities.Select(p => p?.At).Where(d => d.HasValue).DefaultIfEmpty(old.Clients).Max();

        protected override ISinceParameters UpdateSinceParameters(ISinceParameters old, DateTimeOffset? lastUpdated)
            => new SinceParameters(old, clients: lastUpdated);
    }
}
