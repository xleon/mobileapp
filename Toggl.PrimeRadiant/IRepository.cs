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
        IObservable<TModel> Update(long id, TModel entity);
        IObservable<IEnumerable<(ConflictResolutionMode ResolutionMode, TModel Entity)>> BatchUpdate(
            IEnumerable<(long Id, TModel Entity)> entities,
            Func<TModel, TModel, ConflictResolutionMode> conflictResolution);
        IObservable<Unit> Delete(long id);
        IObservable<IEnumerable<TModel>> GetAll();
        IObservable<IEnumerable<TModel>> GetAll(Func<TModel, bool> predicate);
    }
}
