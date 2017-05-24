using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Multivac.Extensions;
using Toggl.Ultrawave.Exceptions;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedEndpointBaseTests<T> : EndpointTestBase
    {
        protected abstract IObservable<T> CallEndpointWith(ITogglClient togglClient);

        protected Func<Task> CallingEndpointWith(ITogglClient togglClient)
            => async () => await CallEndpointWith(togglClient);

        [Fact]
        public async Task WorksWithPassword()
        {
            var credentials = await User.Create();

            CallingEndpointWith(TogglClientWith(credentials)).ShouldNotThrow();
        }

        [Fact]
        public async Task WorksWithApiToken()
        {
            var (_, user) = await SetupTestUser();
            var apiTokenCredentials = Credentials.WithApiToken(user.ApiToken);

            CallingEndpointWith(TogglClientWith(apiTokenCredentials)).ShouldNotThrow();
        }

        [Fact]
        public void FailsForNonExistingUser()
        {
            var email = $"non-existing-email-{Guid.NewGuid()}@ironicmocks.toggl.com".ToEmail();
            var wrongCredentials = Credentials.WithPassword(email, "123456789");

            CallingEndpointWith(TogglClientWith(wrongCredentials)).ShouldThrow<ApiException>();
        }

        [Fact]
        public async Task FailsWithWrongPassword()
        {
            var (email, password) = await User.CreateEmailPassword();
            var wrongCredentials = Credentials.WithPassword(email, $"{password}1");

            CallingEndpointWith(TogglClientWith(wrongCredentials)).ShouldThrow<ApiException>();
        }

        [Fact]
        public void FailsWithWrongApiToken()
        {
            var wrongApiToken = Guid.NewGuid().ToString("N");
            var wrongApiTokenCredentials = Credentials.WithApiToken(wrongApiToken);

            CallingEndpointWith(TogglClientWith(wrongApiTokenCredentials)).ShouldThrow<ApiException>();
        }

        [Fact]
        public void FailsWithoutCredentials()
        {
            CallingEndpointWith(TogglClientWith(Credentials.None)).ShouldThrow<ApiException>();
        }
    }
}
