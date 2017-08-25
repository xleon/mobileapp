using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Realms;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Exceptions;

namespace Toggl.PrimeRadiant.Realm
{
    internal sealed class SingleObjectStorage<TModel> : BaseStorage<TModel>, ISingleObjectStorage<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        public SingleObjectStorage(IRealmAdapter<TModel> adapter)
            : base(adapter) { }
        
        public IObservable<TModel> Create(TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            return Observable.Defer(() =>
            {
                if (Adapter.GetAll().Any())
                    return Observable.Throw<TModel>(new EntityAlreadyExistsException());

                return Adapter.Create(entity)
                              .Apply(Observable.Return)
                              .Catch<TModel, Exception>(ex => Observable.Throw<TModel>(new DatabaseException(ex)));
            });
        }

        public IObservable<TModel> Single()
            => CreateObservable(() => Adapter.GetAll().Single());

        public static SingleObjectStorage<TModel> For<TRealmEntity>(Func<TModel, Realms.Realm, TRealmEntity> convertToRealm)
            where TRealmEntity : RealmObject, TModel, IUpdatesFrom<TModel>
            => new SingleObjectStorage<TModel>(new RealmAdapter<TRealmEntity, TModel>(convertToRealm));

        public IObservable<Unit> Delete()
            => Single().SelectMany(Delete);
    }
}
