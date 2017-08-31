using System;

namespace Toggl.Foundation.Sync.ConflictResolution.Selectors
{
    interface ISyncSelector<T>
    {
        DateTimeOffset LastModified(T model);
        bool IsDirty(T model);
        bool IsDeleted(T model);
    }
}
