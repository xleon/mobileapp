using System;
using System.Reactive.Linq;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Tests.Integration
{
    public abstract class AuthenticatedEndpointWithParameterBaseTests<T, TParameter>
        : AuthenticatedEndpointBaseTests<T>
    {
        protected abstract TParameter GetDefaultParameter(Ultrawave.User user, ITogglClient togglClient);

        protected abstract IObservable<T> CallEndpointWith(ITogglClient togglClient, TParameter parameter);

        protected IObservable<T> CallEndpointWith(Credentials credentials, TParameter parameter)
            => CallEndpointWith(new TogglClient(ApiEnvironment.Staging, credentials), parameter);

        protected Action CallingEndpointWith(ITogglClient togglClient, TParameter parameter)
            => () => CallEndpointWith(togglClient, parameter).Wait();

        protected Action CallingEndpointWith(Credentials credentials, TParameter parameter)
            => () => CallEndpointWith(credentials, parameter).Wait();

        protected sealed override IObservable<T> CallEndpointWith(ITogglClient togglClient)
        {
            var user = togglClient.User.Get().Wait();

            var parameter = GetDefaultParameter(user, togglClient);

            return CallEndpointWith(togglClient, parameter);
        }
    }
}
