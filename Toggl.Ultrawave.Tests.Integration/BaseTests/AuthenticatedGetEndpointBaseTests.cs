using FluentAssertions;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedGetEndpointBaseTests<T> : AuthenticatedEndpointBaseTests<T>
    {
        [Fact]
        public async Task ReturnsTheSameWhetherUsingPasswordOrApiToken()
        {
            var (passwordClient, user) = await SetupTestUser();
            var apiTokenClient = TogglClientWith(Credentials.WithApiToken(user.ApiToken));
            
            var passwordReturn = await CallEndpointWith(passwordClient);
            var apiTokenReturn = await CallEndpointWith(apiTokenClient);

            passwordReturn.ShouldBeEquivalentTo(apiTokenReturn);
        }
    }
}
