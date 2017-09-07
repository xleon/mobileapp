using System;

namespace Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions
{
    public struct TextFieldInfo : IEquatable<TextFieldInfo>
    {
        public string Text { get; }

        public int CursorPosition { get; }

        public string ProjectColor { get; }

        public string ProjectName { get; }

        public int DescriptionCursorPosition
            => Math.Min(CursorPosition, Text.Length);

        public TextFieldInfo(string text, int cursorPosition)
            : this(text, cursorPosition, "", "")
        {
        }

        public TextFieldInfo(string text, int cursorPosition, string projectName, string projectColor)
        {
            Text = text;
            ProjectName = projectName;
            ProjectColor = projectColor;
            CursorPosition = cursorPosition;
        }

        public TextFieldInfo WithTextAndCursor(string text, int cursorPosition)
           => new TextFieldInfo(text, cursorPosition, ProjectName, ProjectColor);

        public TextFieldInfo WithProjectInfo(string name, string color)
           => new TextFieldInfo(Text, CursorPosition, name, color);

        public static bool operator ==(TextFieldInfo left, TextFieldInfo right)
            => left.Equals(right);

        public static bool operator !=(TextFieldInfo left, TextFieldInfo right)
            => !left.Equals(right);

        public override bool Equals(object obj)
            => obj is TextFieldInfo && Equals((TextFieldInfo)obj);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Text.GetHashCode() * 397) ^
                       (CursorPosition.GetHashCode() * 397) ^
                       (ProjectColor.GetHashCode() * 397) ^
                       ProjectName.GetHashCode();
            }
        }

        public bool Equals(TextFieldInfo other)
            => Text == other.Text
            && CursorPosition == other.CursorPosition
            && ProjectName == other.ProjectName
            && ProjectColor == other.ProjectColor;
    }
}
