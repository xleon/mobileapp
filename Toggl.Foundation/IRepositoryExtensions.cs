using System;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation
{
    public static class IRepositoryExtensions
    {
        public static IObservable<TModel> Update<TModel>(this IRepository<TModel> repository, TModel entity)
            where TModel : IBaseModel, IDatabaseSyncable
            => repository.Update(entity.Id, entity);
    }
}
