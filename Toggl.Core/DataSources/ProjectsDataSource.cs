using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
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
