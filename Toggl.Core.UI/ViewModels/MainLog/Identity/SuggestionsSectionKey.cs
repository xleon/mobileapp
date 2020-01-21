namespace Toggl.Core.UI.ViewModels.MainLog.Identity
{
    public class SuggestionsSectionKey : IMainLogKey
    {
        private int identifier = 1048193; // random high prime number to be sure

        public long Identifier()
            => identifier;

        public bool Equals(IMainLogKey otherKey)
            => otherKey is SuggestionsSectionKey;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SuggestionsSectionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return identifier;
        }
    }
}
