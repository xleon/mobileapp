using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class TasksDataSource : DataSource<IThreadSafeTask, IDatabaseTask>
    {
        public TasksDataSource(IRepository<IDatabaseTask> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeTask Convert(IDatabaseTask entity)
            => Task.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseTask first, IDatabaseTask second)
            => Resolver.ForTasks.Resolve(first, second);
    }
}
