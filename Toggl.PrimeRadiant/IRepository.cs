using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant.Repositories
{
    public interface IRepository<TModel>
        where TModel: IBaseModel
    {
        IObservable<TModel> GetLast();
        IObservable<TModel> GetById(int id);
        IObservable<TModel> Create(TModel entity);
        IObservable<TModel> Update(TModel entity);
        IObservable<Unit> Delete(TModel entity);
        IObservable<IEnumerable<TModel>> GetAll(Func<TModel, bool> predicate);
    }
}
