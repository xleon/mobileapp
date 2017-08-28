using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.ConflictResolution.Selectors
{
    internal sealed class TaskSyncSelector : ISyncSelector<IDatabaseTask>
    {
        public DateTimeOffset LastModified(IDatabaseTask model)
            => model.At;

        public bool IsDeleted(IDatabaseTask model)
            => false;
    }
}
