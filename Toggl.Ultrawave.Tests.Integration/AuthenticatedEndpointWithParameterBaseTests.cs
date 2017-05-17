using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

        protected Func<Task> CallingEndpointWith(ITogglClient togglClient, TParameter parameter)
            => async () => await CallEndpointWith(togglClient, parameter);

        protected Func<Task> CallingEndpointWith(Credentials credentials, TParameter parameter)
            => async () => await CallEndpointWith(credentials, parameter);

        protected sealed override IObservable<T> CallEndpointWith(ITogglClient togglClient)
        {
            return Observable.Defer(async () =>
            {
                var user = await togglClient.User.Get();
                var parameter = GetDefaultParameter(user, togglClient);
                
                return CallEndpointWith(togglClient, parameter);
            });
        }
    }
}
