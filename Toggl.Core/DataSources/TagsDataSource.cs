using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class TagsDataSource : DataSource<IThreadSafeTag, IDatabaseTag>
    {
        public TagsDataSource(IRepository<IDatabaseTag> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeTag Convert(IDatabaseTag entity)
            => Tag.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseTag first, IDatabaseTag second)
            => Resolver.ForTags.Resolve(first, second);
    }
}
