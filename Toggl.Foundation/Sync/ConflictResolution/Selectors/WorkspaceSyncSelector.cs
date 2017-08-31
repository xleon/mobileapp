using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.ConflictResolution.Selectors
{
    internal sealed class WorkspaceSyncSelector : ISyncSelector<IDatabaseWorkspace>
    {
        public DateTimeOffset LastModified(IDatabaseWorkspace model)
            => model.At ?? DateTimeOffset.Now;

        public bool IsDirty(IDatabaseWorkspace model)
            => model.IsDirty;

        public bool IsDeleted(IDatabaseWorkspace model)
            => model.ServerDeletedAt.HasValue;
    }
}
