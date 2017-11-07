using System;
using System.Reactive.Linq;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedPutEndpointBaseTests<T> : AuthenticatedEndpointBaseTests<T>
    {
        protected sealed override IObservable<T> CallEndpointWith(ITogglApi api)
            => Observable.Defer(async () =>
            {
                T entityToUpdate;

                try
                {
                    entityToUpdate = await PrepareForCallingUpdateEndpoint(ValidApi);
                }
                catch (ApiException e)
                {
                    throw new InvalidOperationException("Preparation for calling the update endpoint itself failed.", e);
                }

                return CallUpdateEndpoint(api, entityToUpdate);
            });

        protected abstract IObservable<T> PrepareForCallingUpdateEndpoint(ITogglApi api);

        protected abstract IObservable<T> CallUpdateEndpoint(ITogglApi api, T entityToUpdate);
    }
}
