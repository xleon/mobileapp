using System;

namespace Toggl.Storage
{
    public interface IRepository<TModel> : IBaseStorage<TModel>
    {
        IObservable<TModel> GetById(long id);
        IObservable<TModel> ChangeId(long currentId, long newId);
    }
}
