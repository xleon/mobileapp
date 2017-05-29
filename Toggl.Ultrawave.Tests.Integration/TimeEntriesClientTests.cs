using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Tests.Integration.BaseTests;
using Toggl.Ultrawave.Tests.Integration.Helper;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class TimeEntriesClientTests
    {
        public class TheGetAllMethod : AuthenticatedGetEndpointBaseTests<List<TimeEntry>>
        {
            [Fact]
            public async Task ReturnsAllTimeEntries()
            {
                var (togglClient, user) = await SetupTestUser();
                var firstTimeEntry = createTimeEntry(user);
                var firstTimeEntryPosted = await togglClient.TimeEntries.Create(firstTimeEntry);
                var secondTimeEntry = createTimeEntry(user);
                var secondTimeEntryPosted = await togglClient.TimeEntries.Create(secondTimeEntry);

                var timeEntries = await CallEndpointWith(togglClient);

                timeEntries.Should().HaveCount(2);
                timeEntries.Should().Contain(entry =>
                    entry.Description == firstTimeEntryPosted.Description
                    && entry.WorkspaceId == user.DefaultWorkspaceId
                    && entry.Start == firstTimeEntryPosted.Start
                    && entry.UserId == user.Id);
                timeEntries.Should().Contain(entry =>
                    entry.Description == secondTimeEntryPosted.Description
                    && entry.WorkspaceId == user.DefaultWorkspaceId
                    && entry.Start == secondTimeEntryPosted.Start
                    && entry.UserId == user.Id);
            }

            protected override IObservable<List<TimeEntry>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.TimeEntries.GetAll();

            private TimeEntry createTimeEntry(Ultrawave.User user) => new TimeEntry
            {
                WorkspaceId = user.DefaultWorkspaceId,
                Billable = false,
                Start = new DateTimeOffset(DateTime.Now),
                Duration = -1,
                Description = Guid.NewGuid().ToString(),
                Tags = new List<string>(),
                TagIds = new List<int>(),
                At = new DateTimeOffset(DateTime.Now),
                UserId = user.Id,
                CreatedWith = "Ultraware Integration Tests"
            };
        }

        public class TheCreateMethod : AuthenticatedPostEndpointBaseTests<TimeEntry>
        {
            [Fact]
            public async Task CreatesNewClient()
            {
                var (togglClient, user) = await SetupTestUser();
                var newTimeEntry = createTimeEntry(user);

                var persistedTimeEntry = await CallEndpointWith(togglClient, newTimeEntry);

                persistedTimeEntry.Description.Should().Be(newTimeEntry.Description);
                persistedTimeEntry.WorkspaceId.Should().Be(newTimeEntry.WorkspaceId);
                persistedTimeEntry.Billable.Should().Be(false);
                persistedTimeEntry.Duration.Should().BeNegative();
                persistedTimeEntry.ProjectId.Should().BeNull();
                persistedTimeEntry.TaskId.Should().BeNull();
            }
            
            protected override IObservable<TimeEntry> CallEndpointWith(ITogglApi togglApi)
                => Observable.Defer(async () =>
                {
                    var user = await togglApi.User.Get();
                    var timeEntry = createTimeEntry(user);
                    return CallEndpointWith(togglApi, timeEntry);
                });

            private IObservable<TimeEntry> CallEndpointWith(ITogglApi togglApi, TimeEntry client)
                => togglApi.TimeEntries.Create(client);

            private TimeEntry createTimeEntry(Ultrawave.User user) => new TimeEntry
            {
                WorkspaceId = user.DefaultWorkspaceId,
                Billable = false,
                Start = new DateTimeOffset(DateTime.Now),
                Stop = null,
                Duration = -1,
                Description = Guid.NewGuid().ToString(),
                Tags = new List<string>(),
                TagIds = new List<int>(),
                At = new DateTimeOffset(DateTime.Now),
                UserId = user.Id,
                CreatedWith = "Ultraware Integration Tests"
            };
        }
    }
}
