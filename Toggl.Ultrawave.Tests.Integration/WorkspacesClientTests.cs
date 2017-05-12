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
            private readonly TogglClient togglClient = new TogglClient(ApiEnvironment.Staging);

            protected override IObservable<List<Workspace>> CallEndpointWith(
                (string email, string password) credentials)
                => togglClient.Workspaces.GetAll(credentials.email, credentials.password);

            [Fact]
            public async Task ReturnsAllWorkspaces()
            {
                var credentials = await User.Create();
                var user = await togglClient.User.Get(credentials.email, credentials.password);
                var secondWorkspace = await WorkspaceHelper.Create(credentials);

                var workspaces = CallEndpointWith(credentials).Wait();

                workspaces.Should().HaveCount(2);
                workspaces.Should().Contain(ws => ws.Id == user.DefaultWorkspaceId);
                workspaces.Should().Contain(ws => ws.Id == secondWorkspace.Id);
            }
        }

        public class TheGetByIdMethod : AuthenticatedEndpointWithParameterBaseTests<Workspace, int>
        {
            private readonly TogglClient togglClient = new TogglClient(ApiEnvironment.Staging);

            protected override int GetDefaultParameter(Ultrawave.User user, (string email, string password) credentials)
                => user.DefaultWorkspaceId;

            protected override IObservable<Workspace> CallEndpointWith(
                (string email, string password) credentials, int id)
                => togglClient.Workspaces.GetById(credentials.email, credentials.password, id);

            [Fact]
            public async Task ReturnsDefaultWorkspace()
            {
                var credentials = await User.Create();
                var user = await togglClient.User.Get(credentials.email, credentials.password);

                var workspace = CallEndpointWith(credentials, user.DefaultWorkspaceId).Wait();

                workspace.Id.Should().Be(user.DefaultWorkspaceId);
            }

            [Fact]
            public async Task ReturnsCreatedWorkspace()
            {
                var credentials = await User.Create();
                var secondWorkspace = await WorkspaceHelper.Create(credentials);

                var workspace = CallEndpointWith(credentials, secondWorkspace.Id).Wait();

                workspace.Id.Should().Be(secondWorkspace.Id);
                workspace.Name.Should().Be(secondWorkspace.Name);
            }

            [Fact]
            public async Task FailsForWrongWorkspaceId()
            {
                var credentials = await User.Create();
                var user = await togglClient.User.Get(credentials.email, credentials.password);

                CallingEndpointWith(credentials, user.DefaultWorkspaceId - 1).ShouldThrow<ApiException>();
            }
        }
    }
}
