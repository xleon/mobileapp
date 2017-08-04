using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant
{
    public interface IRepository<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        IObservable<TModel> GetById(long id);
        IObservable<TModel> Create(TModel entity);
        IObservable<TModel> Update(TModel entity);
        IObservable<Unit> Delete(TModel entity);
        IObservable<IEnumerable<TModel>> GetAll();
        IObservable<IEnumerable<TModel>> GetAll(Func<TModel, bool> predicate);
    }
}
