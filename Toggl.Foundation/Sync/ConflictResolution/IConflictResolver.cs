using Toggl.PrimeRadiant;

namespace Toggl.Foundation.Sync.ConflictResolution
{
    interface IConflictResolver<T>
    {
        ConflictResolutionMode Resolve(T localEntity, T serverEntity);
    }
}
