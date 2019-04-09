using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Core.DataSources
{
    internal sealed class ProjectsDataSource : DataSource<IThreadSafeProject, IDatabaseProject>
    {
        public ProjectsDataSource(IRepository<IDatabaseProject> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeProject Convert(IDatabaseProject entity)
            => Project.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseProject first, IDatabaseProject second)
            => Resolver.ForProjects.Resolve(first, second);
    }
}
