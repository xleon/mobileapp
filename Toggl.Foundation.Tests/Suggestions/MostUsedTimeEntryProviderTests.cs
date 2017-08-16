using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Suggestions;
using Toggl.Foundation.Tests.Generators;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;
using TimeEntry = Toggl.Ultrawave.Models.TimeEntry;

namespace Toggl.Foundation.Tests.Suggestions
{
    public class MostUsedTimeEntryProviderTests
    {
        public abstract class MostUsedTimeEntryProviderTest
        {
            protected MostUsedTimeEntryProvider Provider { get; }
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
            
            protected MostUsedTimeEntryProviderTest()
            {
                Provider = new MostUsedTimeEntryProvider(Database, TimeService);

                TimeService.CurrentDateTime.Returns(_ => DateTimeOffset.Now);
            }
        }

        public class TheConstructor : MostUsedTimeEntryProviderTest
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDatabase, bool useTimeService)
            {
                var database = useDatabase ? Database : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new MostUsedTimeEntryProvider(database, timeService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheGetSuggestionMethod : MostUsedTimeEntryProviderTest
        {
            [Fact]
            public void ReturnsEmptyObservableIfThereAreNoTimeEntries()
            {
                var isEmpty = Provider.GetSuggestions().IsEmpty().Wait();

                isEmpty.Should().BeTrue();
            }

            [Fact]
            public async Task ReturnsTheMostUsedTimeEntry()
            {
                var timeEntries = new[]
                {
                    createTimeEntry("te0", 0, 0),
                    createTimeEntry("te0", 0, 0),
                    createTimeEntry("te1", 1, 1),
                    createTimeEntry("te1", 1, 2),
                    createTimeEntry("te1", 2, 2)
                };
                var observable = createObservable(timeEntries);
                Database.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(observable);

                var suggestion = await Provider.GetSuggestions();

                suggestion.Description.Should().Be("te0");
                suggestion.TaskId.Should().Be(0);
                suggestion.ProjectId.Should().Be(0);
            }

            private IObservable<IEnumerable<IDatabaseTimeEntry>> createObservable(ITimeEntry[] timeEntries)
            {
                var databaseTimeEntries = timeEntries.Select(Models.TimeEntry.Clean);
                return Observable.Return(databaseTimeEntries);
            }

            private TimeEntry createTimeEntry(string description, int taskId, int projectId)
                => new TimeEntry { Description = description, TaskId = taskId, ProjectId = projectId };
        }
    }
}
