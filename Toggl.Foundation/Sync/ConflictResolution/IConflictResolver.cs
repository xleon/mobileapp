using System;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.Sync.ConflictResolution
{
    interface IConflictResolver<T>
    {
        TimeSpan MarginOfError { get; }
    
        ConflictResolutionMode Resolve(T localEntity, T serverEntity);
    }
}
