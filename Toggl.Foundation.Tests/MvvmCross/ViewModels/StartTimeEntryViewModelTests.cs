using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Xunit;
using TextFieldInfo = Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions.TextFieldInfo;

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

            protected override StartTimeEntryViewModel CreateViewModel()
                => new StartTimeEntryViewModel(DataSource, TimeService, NavigationService);

            protected void SelectProjectViaSuggestion()
            {
                var project = Substitute.For<IDatabaseProject>();
                project.Id.Returns(ProjectId);
                project.Name.Returns(ProjectName);
                project.Color.Returns(ProjectColor);
                var suggestion = new ProjectSuggestionViewModel(project);
                ViewModel.SelectSuggestionCommand.Execute(suggestion);
            }
        }

        public sealed class TheConstructor : StartTimeEntryViewModelTest
        {
            [Theory]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useTimeService, bool useNavigationService)
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
            [Fact]
            public async Task StartProjectSuggestionEvenIfTheProjectHasAlreadyBeenSelected()
            {
                await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                SelectProjectViaSuggestion();

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.IsSuggestingProjects.Should().BeTrue();
                await DataSource.Projects.Received().GetAll();
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
                where TSuggestion : BaseTimeEntrySuggestionViewModel
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

            public sealed class WhenSelectingATimeEntrySuggestion : SelectSuggestionTest<TimeEntrySuggestionViewModel>
            {
                protected override TimeEntrySuggestionViewModel Suggestion { get; }

                public WhenSelectingATimeEntrySuggestion()
                {
                    Suggestion = new TimeEntrySuggestionViewModel(TimeEntry);
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

                    ViewModel.ProjectId.Should().Be(ProjectId);
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

            public sealed class WhenSelectingAProjectSuggestion : SelectSuggestionTest<ProjectSuggestionViewModel>
            {
                protected override ProjectSuggestionViewModel Suggestion { get; }

                public WhenSelectingAProjectSuggestion()
                {
                    Suggestion = new ProjectSuggestionViewModel(Project);

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

                    ViewModel.ProjectId.Should().Be(ProjectId);
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

        public sealed class TheTextFieldInfoProperty : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task RemovesTheProjectIdWhenTheProjectNameIsDeletedFromTheTextField()
            {
                await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                SelectProjectViaSuggestion();

                ViewModel.TextFieldInfo = ViewModel.TextFieldInfo.WithProjectInfo("", "");

                ViewModel.ProjectId.Should().BeNull();
            }

            public sealed class QueriesTheDatabaseForTimeEntries : StartTimeEntryViewModelTest
            {
                [Theory]
                [InlineData("Nothing")]
                [InlineData("Testing Toggl mobile apps")]
                public async Task WhenTheUserBeginsTypingADescription(string description)
                {
                    var textFieldInfo = new TextFieldInfo(description, 0);
                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));

                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.TimeEntries.Received().GetAll();
                }

                [Theory]
                [InlineData("Nothing")]
                [InlineData("Testing Toggl mobile apps")]
                public async Task WhenTheUserHasTypedAnySearchSymbolsButMovedTheCaretToAnIndexThatComesBeforeTheSymbol(
                    string description)
                {
                    var actualDescription = $"{description}@{description}";
                    var textFieldInfo = new TextFieldInfo(actualDescription, 0);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.TimeEntries.Received().GetAll();
                }

                [Fact]
                public async Task WhenTheUserHasAlreadySelectedAProjectAndTypesTheAtSymbol()
                {
                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    SelectProjectViaSuggestion();
                    var description = $"Testing Mobile Apps @toggl";

                    ViewModel.TextFieldInfo = 
                        ViewModel.TextFieldInfo.WithTextAndCursor(description, description.Length);

                    await DataSource.TimeEntries.Received().GetAll();
                }
            }

            public sealed class QueriesTheDatabaseForProjects : StartTimeEntryViewModelTest
            {
                [Theory]
                [InlineData("Nothing")]
                [InlineData("Testing Toggl mobile apps")]
                public async Task WhenTheAtSymbolIsTyped(string description)
                {
                    var actualDescription = $"{description}@{description}";
                    var textFieldInfo = new TextFieldInfo(actualDescription, description.Length + 1);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.Projects.Received().GetAll();
                }
            }
        }

        public sealed class TheQueryMechanism
        {
            public abstract class QueryMechanismTest : StartTimeEntryViewModelTest
            {
                protected IEnumerable<IDatabaseClient> Clients { get; }
                protected IEnumerable<IDatabaseProject> Projects { get; }
                protected IEnumerable<IDatabaseTimeEntry> TimeEntries { get; }

                protected QueryMechanismTest()
                {
                    Clients = Enumerable.Range(10, 10).Select(id =>
                    {
                        var client = Substitute.For<IDatabaseClient>();
                        client.Id.Returns(id);
                        client.Name.Returns(id.ToString());
                        return client;
                    });

                    Projects = Enumerable.Range(20, 10).Select(id =>
                    {
                        var project = Substitute.For<IDatabaseProject>();
                        project.Id.Returns(id);
                        project.Name.Returns(id.ToString());
                        project.Color.Returns("#1e1e1e");

                        var client = id % 2 == 0 ? Clients.Single(c => c.Id == id - 10) : null;
                        project.Client.Returns(client);

                        return project;
                    });

                    TimeEntries = Enumerable.Range(30, 10).Select(id =>
                    {
                        var timeEntry = Substitute.For<IDatabaseTimeEntry>();
                        timeEntry.Id.Returns(id);
                        timeEntry.Description.Returns(id.ToString());

                        var project = id % 2 == 0 ? Projects.Single(c => c.Id == id - 10) : null;
                        timeEntry.Project.Returns(project);

                        return timeEntry;
                    });

                    DataSource.Projects.GetAll()
                        .Returns(callInfo => Observable.Return(Projects));

                    DataSource.TimeEntries.GetAll()
                        .Returns(callInfo => Observable.Return(TimeEntries));
                }
            }

            public sealed class ForTimeEntries : QueryMechanismTest
            {
                [Fact]
                public async Task SearchesTheDescription()
                {
                    const string description = "30";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.TimeEntries.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestionViewModel>();
                }

                [Fact]
                public async Task SearchesTheProjectsName()
                {
                    const string description = "20";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.TimeEntries.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestionViewModel>();
                }

                [Fact]
                public async Task SearchesTheClientsName()
                {
                    const string description = "10";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.TimeEntries.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestionViewModel>();
                }

                [Fact]
                public async Task OnlyDisplaysResultsTheHaveHasAtLeastOneMatchOnEveryWordTyped()
                {
                    const string description = "10 20 3";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.TimeEntries.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestionViewModel>();
                }
            }

            public sealed class ForProjects : QueryMechanismTest
            {

                [Fact]
                public async Task SuggestsNoProjectWhenThereIsNoStringToSearch()
                {
                    const string description = "@";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.Projects.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(11)
                        .And.AllBeOfType<ProjectSuggestionViewModel>();

                    ViewModel.Suggestions.Cast<ProjectSuggestionViewModel>().First()
                        .ProjectName.Should().Be(Resources.NoProject);
                }

                [Fact]
                public async Task SearchesTheName()
                {
                    const string description = "@20";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.Projects.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<ProjectSuggestionViewModel>();
                }

                [Fact]
                public async Task SearchesTheClientsName()
                {
                    const string description = "@10";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.Projects.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<ProjectSuggestionViewModel>();
                }

                [Fact]
                public async Task OnlyDisplaysResultsTheHaveHasAtLeastOneMatchOnEveryWordTyped()
                {
                    const string description = "@10 2";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    await ViewModel.Initialize(DateParameter.WithDate(DateTimeOffset.UtcNow));
                    ViewModel.TextFieldInfo = textFieldInfo;

                    await DataSource.Projects.Received().GetAll();
                    ViewModel.Suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<ProjectSuggestionViewModel>();
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
