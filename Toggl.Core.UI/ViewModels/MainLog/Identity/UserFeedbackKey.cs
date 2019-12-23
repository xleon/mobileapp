using System;

namespace Toggl.Core.UI.ViewModels.MainLog.Identity
{
    internal sealed class UserFeedbackKey : IMainLogKey
    {
        private int identifier = 1048361; // random high prime number to be sure

        public long Identifier()
            => identifier;

        public bool Equals(IMainLogKey otherKey)
            => otherKey is UserFeedbackKey;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is UserFeedbackKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return identifier;
        }
    }
}
