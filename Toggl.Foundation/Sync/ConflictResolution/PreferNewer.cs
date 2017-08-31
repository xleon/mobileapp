using Toggl.Foundation.Sync.ConflictResolution.Selectors;
using Toggl.PrimeRadiant;
using Toggl.Multivac;
using static Toggl.PrimeRadiant.ConflictResolutionMode;

namespace Toggl.Foundation.Sync.ConflictResolution
{
    internal sealed class PreferNewer<T> : IConflictResolver<T>
    {
        private ISyncSelector<T> selector { get; }

        public PreferNewer(ISyncSelector<T> selector)
        {
            Ensure.Argument.IsNotNull(selector, nameof(selector));

            this.selector = selector;
        }

        public ConflictResolutionMode Resolve(T localEntity, T serverEntity)
        {
            Ensure.Argument.IsNotNull(serverEntity, nameof(serverEntity));

            if (selector.IsDeleted(serverEntity))
                return localEntity == null ? Ignore : Delete;

            if (localEntity == null)
                return Create;

            var receivedDataIsOutdated = selector.LastModified(localEntity) > selector.LastModified(serverEntity);
            if (receivedDataIsOutdated)
                return Ignore;

            return Update;
        }
    }
}
