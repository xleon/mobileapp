using System;
using System.Reactive;
using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant
{
    public interface ISingleObjectStorage<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        IObservable<TModel> Single();
        IObservable<Unit> Delete(TModel entity);
        IObservable<TModel> Create(TModel entity);
        IObservable<TModel> Update(TModel entity);
    }
}
