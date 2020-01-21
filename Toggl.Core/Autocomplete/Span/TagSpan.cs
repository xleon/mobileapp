using System;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Autocomplete.Span
{
    public sealed class TagSpan : ISpan
    {
        public long TagId { get; set; }

        public string TagName { get; set; }

        public TagSpan(long tagId, string tagName)
        {
            TagId = tagId;
            TagName = tagName;
        }

        public TagSpan(IThreadSafeTag tag)
            : this(tag.Id, tag.Name)
        {
        }

        public bool Equals(ISpan other)
            => other is TagSpan otherSpan
               && otherSpan.TagId == TagId
               && otherSpan.TagName == TagName;

        public override bool Equals(object obj)
            => Equals(obj as ISpan);

        public override int GetHashCode()
            => HashCode.Combine(TagId, TagName);
    }
}
