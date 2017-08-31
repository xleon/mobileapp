using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.ConflictResolution.Selectors
{
    internal sealed class TimeEntrySyncSelector : ISyncSelector<IDatabaseTimeEntry>
    {
        public DateTimeOffset LastModified(IDatabaseTimeEntry model)
            => model.At;

        public bool IsDirty(IDatabaseTimeEntry model)
            => model.IsDirty;

        public bool IsDeleted(IDatabaseTimeEntry model)
            => model.ServerDeletedAt.HasValue;
    }
}
