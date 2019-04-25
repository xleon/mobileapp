using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Toggl.Networking.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedDeleteEndpointBaseTests<T> : AuthenticatedEndpointBaseTests<T>
    {
        protected sealed override IObservable<T> CallEndpointWith(ITogglApi togglApi)
            => Observable.Defer(async () =>
            {
                var entity = await Initialize(ValidApi);
                await Delete(togglApi, entity);
                return Observable.Return<T>(default(T));
            });

        protected abstract IObservable<T> Initialize(ITogglApi api);
        protected abstract IObservable<Unit> Delete(ITogglApi api, T entity);
    }
}
