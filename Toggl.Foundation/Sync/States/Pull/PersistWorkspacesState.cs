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
    class PersistWorkspacesState : BasePersistState<IWorkspace, IDatabaseWorkspace>
    {
        public PersistWorkspacesState(ITogglDatabase database)
            : base(database)
        {
        }

        protected override IObservable<IEnumerable<IWorkspace>> FetchObservable(FetchObservables fetch)
            => fetch.Workspaces;

        protected override IDatabaseWorkspace ConvertToDatabaseEntity(IWorkspace entity)
            => Workspace.Clean(entity);

        protected override IObservable<IEnumerable<IDatabaseWorkspace>> BatchUpdate(ITogglDatabase database, IEnumerable<(long, IDatabaseWorkspace)> entities)
            => database.Workspaces.BatchUpdate(entities, Resolver.ForWorkspaces().Resolve);

        protected override DateTimeOffset? LastUpdated(ISinceParameters old, IEnumerable<IDatabaseWorkspace> entities)
            => entities.Select(p => p?.At).Where(d => d.HasValue).DefaultIfEmpty(old.Workspaces).Max();

        protected override ISinceParameters UpdateSinceParameters(ISinceParameters old, DateTimeOffset? lastUpdated)
            => new SinceParameters(old, workspaces: lastUpdated);
    }
}
