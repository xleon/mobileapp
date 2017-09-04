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
    class PersistProjectsState : BasePersistState<IProject, IDatabaseProject>
    {
        public PersistProjectsState(ITogglDatabase database)
            : base(database)
        {
        }

        protected override IObservable<IEnumerable<IProject>> FetchObservable(FetchObservables fetch)
            => fetch.Projects;

        protected override IDatabaseProject ConvertToDatabaseEntity(IProject entity)
            => Project.Clean(entity);

        protected override IObservable<IEnumerable<IDatabaseProject>> BatchUpdate(ITogglDatabase database, IEnumerable<IDatabaseProject> entities)
            => database.Projects.BatchUpdate(entities, Resolver.ForProjects().Resolve);

        protected override DateTimeOffset? LastUpdated(ISinceParameters old, IEnumerable<IDatabaseProject> entities)
            => entities.Select(p => p?.At).Where(d => d.HasValue).DefaultIfEmpty(old.Projects).Max();

        protected override ISinceParameters UpdateSinceParameters(ISinceParameters old, DateTimeOffset? lastUpdated)
            => new SinceParameters(old, projects: lastUpdated);
    }
}
