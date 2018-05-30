using System;
using Toggl.Foundation.Models.Interfaces;

namespace Toggl.Foundation.DataSources.Interfaces
{
    public interface ISingletonDataSource<T> : IBaseDataSource<T>
        where T : IThreadSafeModel
    {
        IObservable<T> Current { get; }

        IObservable<T> Get();
    }
}
