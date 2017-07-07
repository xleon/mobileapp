using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;
using TimeEntry = Toggl.Ultrawave.Models.TimeEntry;

namespace Toggl.Foundation.Tests.Suggestions
{
    public class MostUsedTimeEntryProviderTests
    {
        public class TheConstructor
        {
            [Fact]
            public void ThrowsIfArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new MostUsedTimeEntryProvider(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheGetSuggestionMethod
        {
            [Fact]
            public void ReturnsEmptyObservableIfThereAreNoTimeEntries()
            {
                var database = Substitute.For<ITogglDatabase>();
                var provider = new MostUsedTimeEntryProvider(database);

                var isEmpty = provider.GetSuggestion().IsEmpty().Wait();

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
                var database = Substitute.For<ITogglDatabase>();
                database.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(observable);
                var provider = new MostUsedTimeEntryProvider(database);

                var suggestion = await provider.GetSuggestion();

                suggestion.Description.Should().Be("te0");
                suggestion.TaskId.Should().Be(0);
                suggestion.ProjectId.Should().Be(0);
            }

            private IObservable<IEnumerable<IDatabaseTimeEntry>> createObservable(ITimeEntry[] timeEntries)
            {
                var databaseTimeEntries = timeEntries.Select(Foundation.Models.TimeEntry.Clean);
                return Observable.Return(databaseTimeEntries);
            }

            private TimeEntry createTimeEntry(string description, int taskId, int projectId)
                => new TimeEntry { Description = description, TaskId = taskId, ProjectId = projectId };
        }
    }
}
