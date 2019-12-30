namespace Toggl.Core.UI.ViewModels.MainLog.Identity
{
    public class UserFeedbackSectionKey : IMainLogKey
    {
        private int identifier = 1046527; // random high prime number to be sure

        public long Identifier()
            => identifier;

        public bool Equals(IMainLogKey otherKey)
            => otherKey is UserFeedbackSectionKey;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is UserFeedbackSectionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return identifier;
        }
    }
}
