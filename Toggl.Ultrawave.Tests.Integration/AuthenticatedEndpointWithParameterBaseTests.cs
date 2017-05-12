using System;
using System.Reactive.Linq;

namespace Toggl.Ultrawave.Tests.Integration
{
    public abstract class AuthenticatedEndpointWithParameterBaseTests<T, TParameter>
        : AuthenticatedEndpointBaseTests<T>
    {
        protected abstract TParameter GetDefaultParameter(
            Ultrawave.User user, (string email, string password) credentials);

        protected abstract IObservable<T> CallEndpointWith(
            (string email, string password) credentials, TParameter parameter);

        protected Action CallingEndpointWith((string email, string password) credentials, TParameter parameter)
            => () => CallEndpointWith(credentials, parameter).Wait();

        protected sealed override IObservable<T> CallEndpointWith((string email, string password) credentials)
        {
            var user = new TogglClient(ApiEnvironment.Staging)
                .User.Get(credentials.email, credentials.password).Wait();

            var parameter = GetDefaultParameter(user, credentials);

            return CallEndpointWith(credentials, parameter);
        }
    }
}