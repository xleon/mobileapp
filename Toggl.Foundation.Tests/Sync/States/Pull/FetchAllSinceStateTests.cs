using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States
{
    public sealed class FetchAllSinceStateTests
    {
        public sealed class TheStartMethod
        {
            private readonly ITogglDatabase database;
            private readonly ITogglApi api;
            private readonly FetchAllSinceState state;

            public TheStartMethod()
            {
                database = Substitute.For<ITogglDatabase>();
                api = Substitute.For<ITogglApi>();
                state = new FetchAllSinceState(database, api);
            }

            [Fact]
            public void EmitsTransitionToFetchStartedResult()
            {
                var transition = state.Start().SingleAsync().Wait();

                transition.Result.Should().Be(state.FetchStarted);
            }
            
            [Fact]
            public void MakesNoApiCallsBeforeSubscription()
            {
                state.Start();

                var w = api.DidNotReceive().Workspaces;
                var c = api.DidNotReceive().Clients;
                var p = api.DidNotReceive().Projects;
                var t = api.DidNotReceive().TimeEntries;
                var ta = api.DidNotReceive().Tags;
                var ts = api.DidNotReceive().Tasks;
                var wf = api.DidNotReceive().WorkspaceFeatures;
            }

            [Fact]
            public void MakesCorrectCallsWithSinceThresholds()
            {
                var now = DateTimeOffset.Now;
                var sinceParameters = Substitute.For<ISinceParameters>();
                sinceParameters.Clients.Returns(now.AddMinutes(-1));
                sinceParameters.Projects.Returns(now.AddMinutes(-2));
                sinceParameters.Tasks.Returns(now.AddMinutes(-3));
                sinceParameters.Tags.Returns(now.AddMinutes(-4));
                sinceParameters.Workspaces.Returns(now.AddMinutes(-5));
                sinceParameters.TimeEntries.Returns(now.AddMinutes(-6));

                database.SinceParameters.Returns(Substitute.For<ISinceParameterRepository>());
                database.SinceParameters.Get().Returns(sinceParameters);

                state.Start().Wait();
                
                api.Workspaces.Received().GetAll();
                api.Clients.Received().GetAllSince(sinceParameters.Clients.Value);
                api.Projects.Received().GetAllSince(sinceParameters.Projects.Value);
                api.TimeEntries.Received().GetAllSince(sinceParameters.TimeEntries.Value);
                api.Tasks.Received().GetAllSince(sinceParameters.Tasks.Value);
                api.Tags.Received().GetAllSince(sinceParameters.Tags.Value);
            }

            [Fact]
            public void MakesCorrectCallsWithoutSinceThresholds()
            {
                state.Start().Wait();

                api.Workspaces.Received().GetAll();
                api.Clients.Received().GetAll();
                api.Projects.Received().GetAll();
                api.TimeEntries.Received().GetAll();
                api.Tasks.Received().GetAll();
                api.Tags.Received().GetAll();
            }

            [Fact]
            public void ConnectsToApiCallObservables()
            {
                var workspaceCall = false;
                var clientCall = false;
                var projectCall = false;
                var timeEntriesCall = false;
                var taskCall = false;
                var tagCall = false;

                api.Workspaces.GetAll().Returns(Observable.Create<List<IWorkspace>>(o => { workspaceCall = true; return () => { }; }));
                api.Clients.GetAll().Returns(Observable.Create<List<IClient>>(o => { clientCall = true; return () => { }; }));
                api.Projects.GetAll().Returns(Observable.Create<List<IProject>>(o => { projectCall = true; return () => { }; }));
                api.TimeEntries.GetAll().Returns(Observable.Create<List<ITimeEntry>>(o => { timeEntriesCall = true; return () => { }; }));
                api.Tasks.GetAll().Returns(Observable.Create<List<ITask>>(o => { taskCall = true; return () => { }; }));
                api.Tags.GetAll().Returns(Observable.Create<List<ITag>>(o => { tagCall = true; return () => { }; }));

                state.Start().Wait();

                workspaceCall.Should().BeTrue();
                clientCall.Should().BeTrue();
                projectCall.Should().BeTrue();
                timeEntriesCall.Should().BeTrue();
                taskCall.Should().BeTrue();
                tagCall.Should().BeTrue();
            }

            [Fact]
            public void ReturnsReplayingApiCallObservables()
            {
                api.Workspaces.GetAll().Returns(Observable.Return<List<IWorkspace>>(null));
                api.Clients.GetAll().Returns(Observable.Return<List<IClient>>(null));
                api.Projects.GetAll().Returns(Observable.Return<List<IProject>>(null));
                api.TimeEntries.GetAll().Returns(Observable.Return<List<ITimeEntry>>(null));
                api.Tasks.GetAll().Returns(Observable.Return<List<ITask>>(null));
                api.Tags.GetAll().Returns(Observable.Return<List<ITag>>(null));

                var transition = (Transition<FetchObservables>)state.Start().SingleAsync().Wait();

                var observables = transition.Parameter;
                observables.Workspaces.SingleAsync().Wait().Should().BeNull();
                observables.Clients.SingleAsync().Wait().Should().BeNull();
                observables.Projects.SingleAsync().Wait().Should().BeNull();
                observables.TimeEntries.SingleAsync().Wait().Should().BeNull();
                observables.Tasks.SingleAsync().Wait().Should().BeNull();
                observables.Tags.SingleAsync().Wait().Should().BeNull();
            }

            [Fact]
            public void ReturnsSinceParametersFromDatabase()
            {
                var transition = (Transition<FetchObservables>)state.Start().SingleAsync().Wait();

                transition.Parameter.SinceParameters.ShouldBeEquivalentTo(database.SinceParameters.Get(), options => options.IncludingProperties());
            }
        }
    }
}
