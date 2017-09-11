using FluentAssertions;
using FsCheck.Xunit;
using Toggl.Foundation.Autocomplete;
using Xunit;

namespace Toggl.Foundation.Tests.Autocomplete
{
    public sealed class TextFieldInfoTests
    {
        public abstract class TextFieldInfoTest
        {
            protected const long ProjectId = 10;
            protected const string ProjectName = "Toggl";
            protected const string ProjectColor = "#F41F19";
            protected const string Description = "Testing Toggl mobile apps";

            protected TextFieldInfo CreateDefaultTextFieldInfo()
                => new TextFieldInfo(Description, Description.Length, ProjectId, ProjectName, ProjectColor);
        }

        public sealed class TheDescriptionCursorPositionProperty
        {
            [Property]
            public void AlwaysReturnsACursorPositionThatsLessThanOrEqualTheLengthOfTheText(string text, int cursor)
            {
                if (text == null) return;

                var textFieldInfo = new TextFieldInfo(text, cursor);

                textFieldInfo.DescriptionCursorPosition.Should().BeLessOrEqualTo(text.Length);
            }
        }

        public sealed class TheWithTextAndCursorMethod : TextFieldInfoTest
        {
            [Fact]
            public void ChangesOnlyTheTextAndCursorPositionWhileMaintainingTheOtherFields()
            {
                const string newDescription = "Some other text";
                var expected = new TextFieldInfo(
                    newDescription, newDescription.Length,
                    ProjectId, ProjectName, ProjectColor
                );

                var textFieldInfo =
                    CreateDefaultTextFieldInfo()
                        .WithTextAndCursor(newDescription, newDescription.Length);

                textFieldInfo.Should().Be(expected);
            }
        }

        public sealed class TheWithProjectInfoMethod : TextFieldInfoTest
        {
            [Fact]
            public void ChangesOnlyTheProjectRelatedInfoWhileMaintainingTheOtherFields()
            {
                const long newProjectId = 11;
                const string newProjectName = "Some other project";
                const string newProjectColor = "Some other project";
                var expected = new TextFieldInfo(
                    Description, Description.Length,
                    newProjectId, newProjectName, newProjectColor
                );

                var textFieldInfo =
                    CreateDefaultTextFieldInfo()
                        .WithProjectInfo(newProjectId, newProjectName, newProjectColor);

                textFieldInfo.Should().Be(expected);
            }
        }

        public sealed class TheRemoveProjectQueryFromDescriptionIfNeededMethod : TextFieldInfoTest
        {
            [Fact]
            public void RemovesTheProjectQueryIfAnyAtSymbolIsPresent()
            {
                var newDescription = $"{Description}@something";

                var textFieldInfo = new TextFieldInfo(
                    newDescription, newDescription.Length,
                    ProjectId, ProjectName, ProjectColor)
                    .RemoveProjectQueryFromDescriptionIfNeeded();

                textFieldInfo.Text.Should().Be(Description);
            }

            [Fact]
            public void DoesNotChangeAnyPropertyIfThereIsNoProjectQueryInTheDescription()
            {
                var textFieldInfo = CreateDefaultTextFieldInfo();

                var newTextFieldInfo =
                    textFieldInfo.RemoveProjectQueryFromDescriptionIfNeeded();

                textFieldInfo.Should().Be(newTextFieldInfo);
            }
        }
    }
}
