using System;

namespace Toggl.Foundation.Autocomplete
{
    public struct TextFieldInfo : IEquatable<TextFieldInfo>
    {
        public string Text { get; }

        public int CursorPosition { get; }

        public long? ProjectId { get; }

        public string ProjectColor { get; }

        public string ProjectName { get; }

        public int DescriptionCursorPosition
            => Math.Min(CursorPosition, Text.Length);

        public TextFieldInfo(string text, int cursorPosition)
            : this(text, cursorPosition, null, "", "")
        {
        }

        public TextFieldInfo(string text, int cursorPosition, long? projectId, string projectName, string projectColor)
        {
            Text = text;
            ProjectId = projectId;
            ProjectName = projectName;
            ProjectColor = projectColor;
            CursorPosition = cursorPosition;
        }

        public TextFieldInfo WithTextAndCursor(string text, int cursorPosition)
           => new TextFieldInfo(text, cursorPosition, ProjectId, ProjectName, ProjectColor);

        public TextFieldInfo WithProjectInfo(long id, string name, string color)
           => new TextFieldInfo(Text, CursorPosition, id, name, color);

        public TextFieldInfo RemoveProjectQueryFromDescriptionIfNeeded()
        {
            var indexOfProjectQuerySymbol = Text.IndexOf(QuerySymbols.Project);
            if (indexOfProjectQuerySymbol < 0) return this;

            var newText = Text.Substring(0, indexOfProjectQuerySymbol);
            return new TextFieldInfo(newText, newText.Length);
        }

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
