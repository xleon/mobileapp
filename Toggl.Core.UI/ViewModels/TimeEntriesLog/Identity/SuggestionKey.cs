namespace Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity
{
    public class SuggestionKey : IMainLogKey
    {
        private readonly long suggestionId;

        public SuggestionKey(long suggestionId)
        {
            this.suggestionId = suggestionId;
        }

        public long Identifier()
            => suggestionId;

        public bool Equals(IMainLogKey otherKey)
            => otherKey is SuggestionKey other && suggestionId == other.suggestionId;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SuggestionKey other && Equals(other);
        }

        public override int GetHashCode()
            => suggestionId.GetHashCode();
    }
}
