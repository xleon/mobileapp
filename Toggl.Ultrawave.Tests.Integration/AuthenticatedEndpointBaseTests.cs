using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Exceptions;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration
{
    public abstract class AuthenticatedEndpointBaseTests<T>
    {
        protected abstract IObservable<T> CallingEndpointWith((string email, string password) credentials);

        private Action callingEndpointWith((string email, string password) credentials)
            => () => CallingEndpointWith(credentials).Wait();

        [Fact]
        public async Task WorksForExistingUser()
        {
            var credentials = await User.Create();

            callingEndpointWith(credentials).ShouldNotThrow();
        }

        [Fact]
        public void FailsForNonExistingUser()
        {
            var email = $"non-existing-email-{Guid.NewGuid()}@ironicmocks.toggl.com";
            var wrongCredentials = (email, "123456789");

            callingEndpointWith(wrongCredentials).ShouldThrow<ApiException>();

            // TODO: check for error code
        }

        [Fact]
        public async Task FailsIfUsingTheWrongPassword()
        {
            var (email, password) = await User.Create();
            var wrongCredentials = (email, $"{password}1");

            callingEndpointWith(wrongCredentials).ShouldThrow<ApiException>();

            // TODO: check for error code
        }
    }
}