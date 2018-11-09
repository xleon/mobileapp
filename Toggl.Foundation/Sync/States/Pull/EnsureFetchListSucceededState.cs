using System;
using System.Collections.Generic;

namespace Toggl.Foundation.Sync.States.Pull
{
    public sealed class EnsureFetchListSucceededState<T> : EnsureFetchSucceededState<IList<T>>
    {
        protected override IObservable<IList<T>> FetchObservable(IFetchObservables fetch)
            => fetch.GetList<T>();
    }
}
