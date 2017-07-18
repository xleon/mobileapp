using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Tests.Integration.BaseTests;
using Xunit;
using Client = Toggl.Ultrawave.Models.Client;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class ClientsApiTests
    {
        public class TheGetAllMethod : AuthenticatedEndpointBaseTests<List<Client>>
        {
            protected override IObservable<List<Client>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Clients.GetAll();

            [Fact, LogTestInfo]
            public async Task ReturnsAllClients()
            {
                var (togglClient, user) = await SetupTestUser();

                var firstClient = new Client { Name = "First", WorkspaceId = user.DefaultWorkspaceId };
                var firstClientPosted = await togglClient.Clients.Create(firstClient);
                var secondClient = new Client { Name = "Second", WorkspaceId = user.DefaultWorkspaceId };
                var secondClientPosted = await togglClient.Clients.Create(secondClient);

                var clients = await CallEndpointWith(togglClient);

                clients.Should().HaveCount(2);

                clients.Should().Contain(client =>
                    client.Id == firstClientPosted.Id && client.Name == firstClientPosted.Name && client.WorkspaceId == firstClientPosted.WorkspaceId);

                clients.Should().Contain(client =>
                    client.Id == secondClientPosted.Id && client.Name == secondClientPosted.Name && client.WorkspaceId == secondClientPosted.WorkspaceId);
            }
        }

        public class TheCreateMethod : AuthenticatedPostEndpointBaseTests<Client>
        {
            protected override IObservable<Client> CallEndpointWith(ITogglApi togglApi)
                => Observable.Defer(async () =>
                {
                    var user = await togglApi.User.Get();
                    var client = new Client { Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId };
                    return CallEndpointWith(togglApi, client);
                });

            private IObservable<Client> CallEndpointWith(ITogglApi togglApi, Client client)
                => togglApi.Clients.Create(client);

            [Fact, LogTestInfo]
            public async Task CreatesNewClient()
            {
                var (togglClient, user) = await SetupTestUser();
                var newClient = new Client { Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId };

                var persistedClient = await CallEndpointWith(togglClient, newClient);

                persistedClient.Name.Should().Be(newClient.Name);
                persistedClient.WorkspaceId.Should().Be(newClient.WorkspaceId);
            }
        }
    }
}
