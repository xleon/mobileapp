using Toggl.Shared;
using Toggl.PrimeRadiant;
using static Toggl.PrimeRadiant.ConflictResolutionMode;

namespace Toggl.Core.Sync.ConflictResolution
{
    internal sealed class AlwaysOverwrite<T> : IConflictResolver<T>
        where T : class
    {
        public ConflictResolutionMode Resolve(T localEntity, T serverEntity)
        {
            Ensure.Argument.IsNotNull(serverEntity, nameof(serverEntity));

            if (localEntity == null)
                return Create;

            return Update;
        }
    }
}
