using System;

namespace Toggl.Core.Autocomplete.Span
{
    public class TextSpan : ISpan
    {
        public string Text { get; }

        public TextSpan(string text)
        {
            Text = text;
        }

        public bool Equals(ISpan other)
            => other?.GetType() == GetType()
               && other is TextSpan otherSpan
               && otherSpan.Text == Text;

        public override bool Equals(object obj)
            => Equals(obj as ISpan);

        public override int GetHashCode() => HashCode.Combine(Text);
    }
}
