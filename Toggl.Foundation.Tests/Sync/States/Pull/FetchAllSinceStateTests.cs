using System;
using System.Reactive.Linq;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.Helpers;
using Toggl.Foundation.Sync.States.Pull;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;
using Toggl.Ultrawave.ApiClients;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States.Pull
{
    public sealed class FetchAllSinceStateTests
    {
        public sealed class TheStartMethod
        {
            private readonly IRequestSender sender;
            private readonly ITogglApi api;
            private readonly ITimeService timeService;
            private readonly ILeakyBucket leakyBucket;
            private readonly FetchAllSinceState state;
            private readonly DateTimeOffset now = new DateTimeOffset(2017, 02, 15, 13, 50, 00, TimeSpan.Zero);

            public TheStartMethod()
            {
                api = Substitute.For<ITogglApi>();
                sender = Substitute.For<IRequestSender>();
                timeService = Substitute.For<ITimeService>();
                leakyBucket = Substitute.For<ILeakyBucket>();
                leakyBucket.TryClaimFreeSlots(Arg.Any<DateTimeOffset>(), Arg.Any<int>(), out _).Returns(true);
                timeService.CurrentDateTime.Returns(now);
                state = new FetchAllSinceState(api, timeService, sender, leakyBucket);
            }

            [Fact, LogIfTooSlow]
            public void EmitsTransitionToFetchStartedResult()
            {
                var transition = state.Start().SingleAsync().Wait();

                transition.Result.Should().Be(state.FetchStarted);
            }

            [Fact, LogIfTooSlow]
            public void MakesNoApiCallsBeforeSubscription()
            {
                state.Start();

                sender.DidNotReceive().FetchAll<IWorkspace, IWorkspacesApi>(api.Workspaces);
                sender.DidNotReceive()
                    .FetchAll<IWorkspaceFeatureCollection, IWorkspaceFeaturesApi>(api.WorkspaceFeatures);
                sender.DidNotReceive().Fetch<IUser, IUserApi>(api.User);
                sender.DidNotReceive().FetchAllSinceIfPossible<IClient, IDatabaseClient, IClientsApi>(api.Clients);
                sender.DidNotReceive().FetchAllSinceIfPossible<IProject, IDatabaseProject, IProjectsApi>(api.Projects);
                sender.DidNotReceive().FetchTimeEntries(api.TimeEntries);
                sender.DidNotReceive().FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api.Tags);
                sender.DidNotReceive().FetchAllSinceIfPossible<ITask, IDatabaseTask, ITasksApi>(api.Tasks);
                sender.DidNotReceive().Fetch<IPreferences, IPreferencesApi>(api.Preferences);
            }

            [Property]
            public void ReturnsPreventServerOverloadWithCorrectDelayWhenTheLeakyBucketIsFull(TimeSpan delay)
            {
                leakyBucket.TryClaimFreeSlots(Arg.Any<DateTimeOffset>(), Arg.Any<int>(), out _)
                    .Returns(x =>
                    {
                        x[2] = delay;
                        return false;
                    });

                var transition = state.Start().SingleAsync().Wait();

                transition.Result.Should().Be(state.PreventOverloadingServer);
                var parameter = ((Transition<TimeSpan>)transition).Parameter;
                parameter.Should().Be(delay);
            }
        }
    }
}
