using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.ConflictResolution.Selectors
{
    internal sealed class ProjectSyncSelector : ISyncSelector<IDatabaseProject>
    {
        public DateTimeOffset LastModified(IDatabaseProject model)
            => model.At;

        public bool IsDeleted(IDatabaseProject model)
            => model.ServerDeletedAt.HasValue;
    }
}
