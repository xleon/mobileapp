using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Exceptions;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration
{
    public abstract class AuthenticatedEndpointBaseTests<T> : EndpointTestBase
    {
        protected abstract IObservable<T> CallEndpointWith(ITogglClient togglClient);

        protected Action CallingEndpointWith(ITogglClient togglClient)
            => () => CallEndpointWith(togglClient).Wait();

        [Fact]
        public async Task WorksForExistingUser()
        {
            var credentials = await User.Create();

            CallingEndpointWith(TogglClientWith(credentials)).ShouldNotThrow();
        }

        [Fact]
        public void FailsForNonExistingUser()
        {
            var email = $"non-existing-email-{Guid.NewGuid()}@ironicmocks.toggl.com";
            var wrongCredentials = Credentials.WithPassword(email, "123456789");

            CallingEndpointWith(TogglClientWith(wrongCredentials)).ShouldThrow<ApiException>();

            // TODO: check for error code
        }

        [Fact]
        public async Task FailsIfUsingTheWrongPassword()
        {
            var (email, password) = await User.CreateEmailPassword();
            var wrongCredentials = Credentials.WithPassword(email, $"{password}1");

            CallingEndpointWith(TogglClientWith(wrongCredentials)).ShouldThrow<ApiException>();

            // TODO: check for error code
        }
    }
}
