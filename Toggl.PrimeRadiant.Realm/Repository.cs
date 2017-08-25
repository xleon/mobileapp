using System;
using System.Linq;
using System.Reactive.Linq;
using Realms;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Exceptions;

namespace Toggl.PrimeRadiant.Realm
{
    internal sealed class Repository<TModel> : BaseStorage<TModel>, IRepository<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        public Repository(IRealmAdapter<TModel> adapter)
            : base(adapter) { }

        public IObservable<TModel> Create(TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            return Observable
                .Start(() => Adapter.Create(entity))
                .Catch<TModel, Exception>(ex => Observable.Throw<TModel>(new DatabaseException(ex)));
        }

        public IObservable<TModel> GetById(long id)
            => CreateObservable(() => Adapter.GetAll().Single(x => x.Id == id));

        public static Repository<TModel> For<TRealmEntity>(Func<TModel, Realms.Realm, TRealmEntity> convertToRealm)
            where TRealmEntity : RealmObject, TModel, IUpdatesFrom<TModel>
            => new Repository<TModel>(new RealmAdapter<TRealmEntity, TModel>(convertToRealm));
    }
}
