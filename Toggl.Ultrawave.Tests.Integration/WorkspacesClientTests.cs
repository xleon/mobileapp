using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Exceptions;
using Toggl.Ultrawave.Tests.Integration.Helper;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class WorkspacesClientTests
    {
        public class TheGetMethod : AuthenticatedEndpointBaseTests<List<Workspace>>
        {
            protected override IObservable<List<Workspace>> CallEndpointWith(ITogglClient togglClient)
                => togglClient.Workspaces.GetAll();

            [Fact]
            public async Task ReturnsAllWorkspaces()
            {
                var (togglClient, user) = await SetupTestUser();
                var secondWorkspace = await WorkspaceHelper.CreateFor(user);

                var workspaces = await CallEndpointWith(togglClient);

                workspaces.Should().HaveCount(2);
                workspaces.Should().Contain(ws => ws.Id == user.DefaultWorkspaceId);
                workspaces.Should().Contain(ws => ws.Id == secondWorkspace.Id);
            }
        }

        public class TheGetByIdMethod : AuthenticatedEndpointWithParameterBaseTests<Workspace, int>
        {
            protected override int GetDefaultParameter(Ultrawave.User user, ITogglClient togglClient)
                => user.DefaultWorkspaceId;

            protected override IObservable<Workspace> CallEndpointWith(ITogglClient togglClient, int id)
                => togglClient.Workspaces.GetById(id);

            [Fact]
            public async Task ReturnsDefaultWorkspace()
            {
                var (togglClient, user) = await SetupTestUser();

                var workspace = await CallEndpointWith(togglClient, user.DefaultWorkspaceId);

                workspace.Id.Should().Be(user.DefaultWorkspaceId);
            }

            [Fact]
            public async Task ReturnsCreatedWorkspace()
            {
                var (togglClient, user) = await SetupTestUser();
                var secondWorkspace = await WorkspaceHelper.CreateFor(user);

                var workspace = await CallEndpointWith(togglClient, secondWorkspace.Id);

                workspace.Id.Should().Be(secondWorkspace.Id);
                workspace.Name.Should().Be(secondWorkspace.Name);
            }

            [Fact]
            public async Task FailsForWrongWorkspaceId()
            {
                var (togglClient, user) = await SetupTestUser();

                CallingEndpointWith(togglClient, user.DefaultWorkspaceId - 1).ShouldThrow<ApiException>();
            }
        }
    }
}
