using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.Autocomplete
{
    public sealed class AutocompleteProviderTests
    {
        public abstract class AutocompleteProviderTest
        {
            protected const long ProjectId = 10;
            protected const string ProjectName = "Toggl";
            protected const string ProjectColor = "#F41F19";
            protected const string Description = "Testing Toggl mobile apps";

            protected AutocompleteProvider Provider { get; }

            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();

            protected IEnumerable<IDatabaseClient> Clients { get; }
            protected IEnumerable<IDatabaseProject> Projects { get; }
            protected IEnumerable<IDatabaseTimeEntry> TimeEntries { get; }

            protected AutocompleteProviderTest()
            {
                Provider = new AutocompleteProvider(Database);

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

                Database.Projects.GetAll()
                    .Returns(callInfo => Observable.Return(Projects));

                Database.TimeEntries.GetAll()
                    .Returns(callInfo => Observable.Return(TimeEntries));
            }
        }

        public sealed class TheConstructor
        {
            [Fact]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new AutocompleteProvider(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheQueryMethod
        {
            public sealed class QueriesTheDatabaseForTimeEntries : AutocompleteProviderTest
            {
                [Theory]
                [InlineData("Nothing")]
                [InlineData("Testing Toggl mobile apps")]
                public async Task WhenTheUserBeginsTypingADescription(string description)
                {
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    await Provider.Query(textFieldInfo);

                    await Database.TimeEntries.Received().GetAll();
                }

                [Theory]
                [InlineData("Nothing")]
                [InlineData("Testing Toggl mobile apps")]
                public async Task WhenTheUserHasTypedAnySearchSymbolsButMovedTheCaretToAnIndexThatComesBeforeTheSymbol(
                    string description)
                {
                    var actualDescription = $"{description}@{description}";
                    var textFieldInfo = new TextFieldInfo(actualDescription, 0);

                    await Provider.Query(textFieldInfo);

                    await Database.TimeEntries.Received().GetAll();
                }

                [Fact]
                public async Task WhenTheUserHasAlreadySelectedAProjectAndTypesTheAtSymbol()
                {
                    var description = $"Testing Mobile Apps @toggl";
                    var textFieldInfo = new TextFieldInfo(
                        description, description.Length,
                        ProjectId, ProjectName, ProjectColor);
                    
                    await Provider.Query(textFieldInfo);

                    await Database.TimeEntries.Received().GetAll();
                }

                [Fact]
                public async Task SearchesTheDescription()
                {
                    const string description = "30";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.TimeEntries.Received().GetAll();
                    suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestion>();
                }

                [Fact]
                public async Task SearchesTheProjectsName()
                {
                    const string description = "20";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.TimeEntries.Received().GetAll();
                    suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestion>();
                }

                [Fact]
                public async Task SearchesTheClientsName()
                {
                    const string description = "10";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.TimeEntries.Received().GetAll();
                    suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestion>();
                }

                [Fact]
                public async Task OnlyDisplaysResultsTheHaveHasAtLeastOneMatchOnEveryWordTyped()
                {
                    const string description = "10 20 3";
                    var textFieldInfo = new TextFieldInfo(description, 0);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.TimeEntries.Received().GetAll();
                    suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<TimeEntrySuggestion>();
                }
            }

            public sealed class QueriesTheDatabaseForProjects : AutocompleteProviderTest
            {
                [Theory]
                [InlineData("Nothing")]
                [InlineData("Testing Toggl mobile apps")]
                public async Task WhenTheAtSymbolIsTyped(string description)
                {
                    var actualDescription = $"{description}@{description}";
                    var textFieldInfo = new TextFieldInfo(actualDescription, description.Length + 1);

                    await Provider.Query(textFieldInfo);

                    await Database.Projects.Received().GetAll();
                }

                [Fact]
                public async Task SuggestsNoProjectWhenThereIsNoStringToSearch()
                {
                    const string description = "@";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.Projects.Received().GetAll();
                    suggestions.Should().HaveCount(11)
                        .And.AllBeOfType<ProjectSuggestion>();

                    suggestions.Cast<ProjectSuggestion>().First()
                        .ProjectName.Should().Be(Resources.NoProject);
                }

                [Fact]
                public async Task SearchesTheName()
                {
                    const string description = "@20";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.Projects.Received().GetAll();
                    suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<ProjectSuggestion>();
                }

                [Fact]
                public async Task SearchesTheClientsName()
                {
                    const string description = "@10";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.Projects.Received().GetAll();
                    suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<ProjectSuggestion>();
                }

                [Fact]
                public async Task OnlyDisplaysResultsTheHaveHasAtLeastOneMatchOnEveryWordTyped()
                {
                    const string description = "@10 2";
                    var textFieldInfo = new TextFieldInfo(description, 1);

                    var suggestions = await Provider.Query(textFieldInfo);

                    await Database.Projects.Received().GetAll();
                    suggestions.Should().HaveCount(1)
                        .And.AllBeOfType<ProjectSuggestion>();
                }
            }
        }
    }
}