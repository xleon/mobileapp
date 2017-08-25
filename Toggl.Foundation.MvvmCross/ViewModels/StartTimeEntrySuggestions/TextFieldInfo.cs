using System;

namespace Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions
{
    public struct TextFieldInfo : IEquatable<TextFieldInfo>
    {
        public string Text { get; }

        public int CursorPosition { get; }

        public TextFieldInfo(string text, int cursorPosition)
        {
            Text = text;
            CursorPosition = cursorPosition;
        }

        public static bool operator ==(TextFieldInfo left, TextFieldInfo right)
            => left.Equals(right);

        public static bool operator !=(TextFieldInfo left, TextFieldInfo right)
            => !left.Equals(right);

        public override bool Equals(object obj)
            => obj is TextFieldInfo && Equals((TextFieldInfo)obj);

        public override int GetHashCode()
            => (Text + CursorPosition).GetHashCode();

        public bool Equals(TextFieldInfo other)
            => Text == other.Text && CursorPosition == other.CursorPosition;
    }
}
