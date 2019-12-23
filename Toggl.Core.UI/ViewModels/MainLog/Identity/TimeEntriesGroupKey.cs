using System;

namespace Toggl.Core.UI.ViewModels.MainLog.Identity
{
    internal sealed class TimeEntriesGroupKey : IMainLogKey
    {
        private readonly GroupId groupId;

        public TimeEntriesGroupKey(GroupId groupId)
        {
            this.groupId = groupId;
        }

        public long Identifier()
            => groupId?.GetHashCode() ?? 0; // This can result in non-unique identifiers

        public bool Equals(IMainLogKey other)
            => other is TimeEntriesGroupKey groupKey && groupId.Equals(groupKey.groupId);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TimeEntriesGroupKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return groupId.GetHashCode();
        }
    }
}
