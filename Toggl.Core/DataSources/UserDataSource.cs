using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class UserDataSource : SingletonDataSource<IThreadSafeUser, IDatabaseUser>
    {
        public UserDataSource(ISingleObjectStorage<IDatabaseUser> storage)
            : base(storage, null)
        {
        }

        protected override IThreadSafeUser Convert(IDatabaseUser entity)
            => User.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseUser first, IDatabaseUser second)
            => Resolver.ForUser.Resolve(first, second);
    }
}
