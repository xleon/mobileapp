using Toggl.PrimeRadiant;

namespace Toggl.Core.Sync.ConflictResolution
{
    internal interface IConflictResolver<T>
    {
        ConflictResolutionMode Resolve(T localEntity, T serverEntity);
    }
}
