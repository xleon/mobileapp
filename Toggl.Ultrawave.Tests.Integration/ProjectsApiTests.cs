using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Models;
using Toggl.Ultrawave.Tests.Integration.BaseTests;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class ProjectsApiTests
    {
        public class TheGetAllMethod : AuthenticatedEndpointBaseTests<List<Project>>
        {
            protected override IObservable<List<Project>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Projects.GetAll();

            [Fact, LogTestInfo]
            public async System.Threading.Tasks.Task ReturnsAllProjects()
            {
                var (togglClient, user) = await SetupTestUser();

                var projectA = await createNewProject(togglClient, user.DefaultWorkspaceId, createClient: true);
                var projectAPosted = await togglClient.Projects.Create(projectA);

                var projectB = await createNewProject(togglClient, user.DefaultWorkspaceId);
                var projectBPosted = await togglClient.Projects.Create(projectB);

                var projects = await CallEndpointWith(togglClient);

                projects.Should().HaveCount(2);

                projects.Should().Contain(project => isTheSameAs(projectAPosted, project));
                projects.Should().Contain(project => isTheSameAs(projectBPosted, project));
            }

            [Fact, LogTestInfo]
            public async System.Threading.Tasks.Task ReturnsOnlyActiveProjects()
            {
                var (togglClient, user) = await SetupTestUser();

                var activeProject = await createNewProject(togglClient, user.DefaultWorkspaceId);
                var activeProjectPosted = await togglClient.Projects.Create(activeProject);

                var inactiveProject = await createNewProject(togglClient, user.DefaultWorkspaceId, isActive: false);
                var inactiveProjectPosted = await togglClient.Projects.Create(inactiveProject);

                var projects = await CallEndpointWith(togglClient);

                projects.Should().HaveCount(1);
                projects.Should().Contain(project => isTheSameAs(project, activeProjectPosted));
                projects.Should().NotContain(project => isTheSameAs(project, inactiveProjectPosted));
            }

            [Fact, LogTestInfo]
            public async System.Threading.Tasks.Task ReturnsNullOnEmptyResultSet()
            {
                var (togglClient, user) = await SetupTestUser();

                var noProjects = await CallEndpointWith(togglClient);

                Project project = await createNewProject(togglClient, user.DefaultWorkspaceId, isActive: false);
                await togglClient.Projects.Create(project);

                project = await createNewProject(togglClient, user.DefaultWorkspaceId, isActive: false);
                await togglClient.Projects.Create(project);

                var activeProjects = await CallEndpointWith(togglClient);

                noProjects.Should().BeNull();
                activeProjects.Should().BeNull();
            }

            public class TheCreateMethod : AuthenticatedPostEndpointBaseTests<Project>
            {
                protected override IObservable<Project> CallEndpointWith(ITogglApi togglApi)
                    => Observable.Defer(async () =>
                    {
                        var user = await togglApi.User.Get();
                        var project = await createNewProject(togglApi, user.DefaultWorkspaceId);
                        return CallEndpointWith(togglApi, project);
                    });

                private IObservable<Project> CallEndpointWith(ITogglApi togglApi, Project project)
                    => togglApi.Projects.Create(project);

                [Fact, LogTestInfo]
                public async System.Threading.Tasks.Task CreatesNewProject()
                {
                    var (togglClient, user) = await SetupTestUser();

                    var project = await createNewProject(togglClient, user.DefaultWorkspaceId);
                    var persistedProject = await CallEndpointWith(togglClient, project);

                    persistedProject.Name.Should().Be(project.Name);
                    persistedProject.ClientId.Should().Be(project.ClientId);
                    persistedProject.IsPrivate.Should().Be(project.IsPrivate);
                    persistedProject.Color.Should().Be(project.Color);
                }
            }

            private static async Task<Project> createNewProject(ITogglApi togglClient, int workspaceID, bool isActive = true, bool createClient = false)
            {
                Client client = null;

                if (createClient)
                {
                    client = new Client
                    {
                        Name = Guid.NewGuid().ToString(),
                        WorkspaceId = workspaceID
                    };

                    client = await togglClient.Clients.Create(client);
                }

                return new Project
                {
                    Name = Guid.NewGuid().ToString(),
                    WorkspaceId = workspaceID,
                    At = DateTimeOffset.UtcNow,
                    Color = "#06aaf5",
                    Active = isActive,
                    ClientId = client?.Id
                };
            }

            private static bool isTheSameAs(Project a, Project b)
                => a.Id == b.Id
                && a.Name == b.Name
                && a.ClientId == b.ClientId
                && a.IsPrivate == b.IsPrivate
                && a.Color == b.Color;
        }
    }
}
