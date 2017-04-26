using System;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Clients;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class UserClientTests
    {
        public class TheGetMethod
        {
            private readonly IUserClient userClient = new TogglClient(ApiEnvironment.Staging).User;

            [Fact]
            public async Task WorksForExistingUsers()
            {
                var (email, password) = await User.Create();

                var response = await userClient.Get(email, password).Execute();

                response.Success.Should().BeTrue();
            }

            [Fact]
            public async Task FailsForNonExistingEmails()
            {
                var response = await userClient.Get($"some-non-existing-email-{Guid.NewGuid()}@ironicmocks.toggl.com", "123456789").Execute();

                response.Success.Should().BeFalse();
                //TODO: Include check expected error message/code
            }

            [Fact]
            public async Task FailsIfUsingTheWrongPassword()
            {
                var (email, password) = await User.Create();

                var response = await userClient.Get(email, "123457").Execute();

                response.Success.Should().BeFalse();
                //TODO: Include check expected error message/code
            }
        }
    }
}