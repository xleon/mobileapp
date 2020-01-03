using System;

namespace Toggl.Core.Autocomplete.Span
{
    public interface ISpan : IEquatable<ISpan>
    {
    }

    public static class SpanExtensions
    {
        public static bool IsTextSpan(this ISpan span) => span is TextSpan;
    }
}
