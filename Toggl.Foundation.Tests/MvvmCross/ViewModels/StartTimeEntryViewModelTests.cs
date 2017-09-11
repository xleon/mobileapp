using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Xunit;
using TextFieldInfo = Toggl.Foundation.Autocomplete.TextFieldInfo;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class StartTimeEntryViewModelTests
    {
        public abstract class StartTimeEntryViewModelTest : BaseViewModelTests<StartTimeEntryViewModel>
        {
            protected const long ProjectId = 10;
            protected const string ProjectName = "Toggl";
            protected const string ProjectColor = "#F41F19";
            protected const string Description = "Testing Toggl mobile apps";

            protected IAutocompleteProvider AutocompleteProvider { get; } = Substitute.For<IAutocompleteProvider>();

            protected StartTimeEntryViewModelTest()
            {
                DataSource.AutocompleteProvider.Returns(AutocompleteProvider);
            }

            protected override StartTimeEntryViewModel CreateViewModel()
                => new StartTimeEntryViewModel(DataSource, TimeService, NavigationService);
        }

        public sealed class TheConstructor : StartTimeEntryViewModelTest
        {
            [Theory]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useTimeService, 
                bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new StartTimeEntryViewModel(dataSource, timeService, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task SetsTheDateAccordingToTheDateParameterReceived()
            {
                var date = DateTimeOffset.UtcNow;
                var parameter = DateParameter.WithDate(date);

                await ViewModel.Initialize(parameter);

                ViewModel.StartDate.Should().BeSameDateAs(date);
            }
        }

        public sealed class TheBackCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.BackCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }

        public sealed class TheToggleBillableCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public void TogglesTheIsBillableProperty()
            {
                var expected = !ViewModel.IsBillable;

                ViewModel.ToggleBillableCommand.Execute();

                ViewModel.IsBillable.Should().Be(expected);
            }
        }

        public sealed class TheToggleProjectSuggestionsCommandCommand : StartTimeEntryViewModelTest
        {
            public TheToggleProjectSuggestionsCommandCommand()
            {
                var items = ProjectSuggestion.FromProjectsPrependingEmpty(Enumerable.Empty<IDatabaseProject>());
                AutocompleteProvider
                    .Query(Arg.Is<TextFieldInfo>(info => info.Text.Contains("@")))
                    .Returns(Observable.Return(items));
            }

            [Fact]
            public async Task StartProjectSuggestionEvenIfTheProjectHasAlreadyBeenSelected()
            {
                await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                ViewModel.TextFieldInfo = new TextFieldInfo(
                    Description, Description.Length,
                    ProjectId, ProjectName, ProjectColor);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.IsSuggestingProjects.Should().BeTrue();
            }

            [Fact]
            public async Task SetsTheIsSuggestingProjectsPropertyToTrueIfNotInProjectSuggestionMode()
            {
                await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.IsSuggestingProjects.Should().BeTrue();
            }

            [Fact]
            public async Task AddsAnAtSymbolAtTheEndOfTheQueryInOrderToStartProjectSuggestionMode()
            {
                const string description = "Testing Toggl Apps";
                var expected = $"{description}@";
                await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                ViewModel.TextFieldInfo = new TextFieldInfo(description, description.Length);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.TextFieldInfo.Text.Should().Be(expected);
            }

            [Theory]
            [InlineData("@")]
            [InlineData("@somequery")]
            [InlineData("@some query")]
            [InlineData("@some query@query")]
            [InlineData("Testing Toggl Apps @")]
            [InlineData("Testing Toggl Apps @somequery")]
            [InlineData("Testing Toggl Apps @some query")]
            [InlineData("Testing Toggl Apps @some query@query")]
            [InlineData("Testing Toggl Apps@")]
            [InlineData("Testing Toggl Apps@somequery")]
            [InlineData("Testing Toggl Apps@some query")]
            [InlineData("Testing Toggl Apps@some query@query")]
            public async Task SetsTheIsSuggestingProjectsPropertyToFalseIfInProjectSuggestionMode(string description)
            {
                await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                ViewModel.TextFieldInfo = new TextFieldInfo(description, description.Length);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.IsSuggestingProjects.Should().BeFalse();
            }

            [Theory]
            [InlineData("@", "")]
            [InlineData("@somequery", "")]
            [InlineData("@some query", "")]
            [InlineData("@some query@query", "")]
            [InlineData("Testing Toggl Apps @", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @somequery", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @some query", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @some query@query", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps@", "Testing Toggl Apps")]
            [InlineData("Testing Toggl Apps@somequery", "Testing Toggl Apps")]
            [InlineData("Testing Toggl Apps@some query", "Testing Toggl Apps")]
            [InlineData("Testing Toggl Apps@some query@query", "Testing Toggl Apps")]
            public async Task RemovesTheAtSymbolFromTheDescriptionTextIfAlreadyInProjectSuggestionMode(
                string description, string expected)
            {
                await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                ViewModel.TextFieldInfo = new TextFieldInfo(description, description.Length);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.TextFieldInfo.Text.Should().Be(expected);
            }
        }

        public sealed class TheDoneCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task StartsANewTimeEntry()
            {
                var date = DateTimeOffset.UtcNow;
                var description = "Testing Toggl apps";
                var dateParameter = DateParameter.WithDate(date);

                await ViewModel.Initialize(dateParameter);
                ViewModel.TextFieldInfo = new TextFieldInfo(description, 0);
                ViewModel.DoneCommand.Execute();

                await DataSource.TimeEntries.Received().Start(
                    Arg.Is(dateParameter.GetDate()),
                    Arg.Is(description),
                    Arg.Is(false),
                    Arg.Is<long?>(x => x == null)
                );
            }

            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.DoneCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }

        public sealed class TheSelectSuggestionCommand
        {
            public abstract class SelectSuggestionTest<TSuggestion> : StartTimeEntryViewModelTest
                where TSuggestion : AutocompleteSuggestion
            {
                protected IDatabaseProject Project { get; }
                protected IDatabaseTimeEntry TimeEntry { get; }

                protected abstract TSuggestion Suggestion { get; }

                protected SelectSuggestionTest()
                {
                    Project = Substitute.For<IDatabaseProject>();
                    Project.Id.Returns(ProjectId);
                    Project.Name.Returns(ProjectName);
                    Project.Color.Returns(ProjectColor);
                    TimeEntry = Substitute.For<IDatabaseTimeEntry>();
                    TimeEntry.Description.Returns(Description);
                    TimeEntry.Project.Returns(Project);
                }
            }

            public sealed class WhenSelectingATimeEntrySuggestion : SelectSuggestionTest<TimeEntrySuggestion>
            {
                protected override TimeEntrySuggestion Suggestion { get; }

                public WhenSelectingATimeEntrySuggestion()
                {
                    Suggestion = new TimeEntrySuggestion(TimeEntry);
                }

                [Fact]
                public void SetsTheTextFieldInfoTextToTheValueOfTheSuggestedDescription()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.Text.Should().Be(Description);
                }

                [Fact]
                public void SetsTheProjectIdToTheSuggestedProjectId()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectId.Should().Be(ProjectId);
                }

                [Fact]
                public void SetsTheProjectNameToTheSuggestedProjectName()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectName.Should().Be(ProjectName);
                }

                [Fact]
                public void SetsTheProjectColorToTheSuggestedProjectColor()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectColor.Should().Be(ProjectColor);
                }
            }

            public sealed class WhenSelectingAProjectSuggestion : SelectSuggestionTest<ProjectSuggestion>
            {
                protected override ProjectSuggestion Suggestion { get; }

                public WhenSelectingAProjectSuggestion()
                {
                    Suggestion = new ProjectSuggestion(Project);

                    ViewModel.TextFieldInfo = new TextFieldInfo("Something @togg", 15);
                }

                [Fact]
                public void RemovesTheProjectQueryFromTheTextFieldInfo()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.Text.Should().Be("Something ");
                }

                [Fact]
                public void SetsTheProjectIdToTheSuggestedProjectId()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectId.Should().Be(ProjectId);
                }

                [Fact]
                public void SetsTheProjectNameToTheSuggestedProjectName()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectName.Should().Be(ProjectName);
                }

                [Fact]
                public void SetsTheProjectColorToTheSuggestedProjectColor()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectColor.Should().Be(ProjectColor);
                }
            }
        }

        public sealed class TheSuggestionsProperty : StartTimeEntryViewModelTest
        {
            [Fact]
            public void IsClearedWhenThereAreNoWordsToQuery()
            {
                ViewModel.TextFieldInfo = new TextFieldInfo("", 0);

                ViewModel.Suggestions.Should().HaveCount(0);
            }
        }
    }
}
