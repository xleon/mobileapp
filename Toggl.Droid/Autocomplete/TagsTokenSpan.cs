using Android.Graphics;

namespace Toggl.Droid.Autocomplete
{
    public sealed class TagsTokenSpan : TokenSpan
    {
        public long TagId { get; }

        public string TagName { get; }

        public TagsTokenSpan(long tagId, string tagName)
            : base(Color.White, Color.White, true)
        {
            TagId = tagId;
            TagName = tagName;
        }
    }
}
