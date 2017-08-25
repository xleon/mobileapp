using System;
using System.Linq;
using Realms;
using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal interface IRealmAdapter<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        IQueryable<TModel> GetAll();

        void Delete(TModel entity);

        TModel Create(TModel entity);

        TModel Update(TModel entity);   
    }

    internal sealed class RealmAdapter<TRealmEntity, TModel> : IRealmAdapter<TModel>
        where TRealmEntity : RealmObject, TModel, IUpdatesFrom<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        private readonly Func<TModel, Realms.Realm, TRealmEntity> convertToRealm;

        public RealmAdapter(Func<TModel, Realms.Realm, TRealmEntity> convertToRealm)
        {
            Ensure.Argument.IsNotNull(convertToRealm, nameof(convertToRealm));

            this.convertToRealm = convertToRealm;
        }

        public IQueryable<TModel> GetAll()
            => Realms.Realm.GetInstance().All<TRealmEntity>();

        public TModel Create(TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            return doTransaction(realm => realm.Add(convertToRealm(entity, realm)));
        }
        
        public TModel Update(TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            return doModyfingTransaction(entity, (realm, realmEntity) => realmEntity.SetPropertiesFrom(entity, realm));
        }

        public void Delete(TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            doModyfingTransaction(entity, (realm, realmEntity) => realm.Remove(realmEntity));
        }

        private static TModel doModyfingTransaction(TModel entity, Action<Realms.Realm, TRealmEntity> transact)
            => doTransaction(realm =>
            {
                var realmEntity = realm.All<TRealmEntity>().Single(x => x.Id == entity.Id);
                transact(realm, realmEntity);
                return realmEntity;
            });

        private static TModel doTransaction(Func<Realms.Realm, TModel> transact)
        {
            var realm = Realms.Realm.GetInstance();
            using (var transaction = realm.BeginWrite())
            {
                var returnValue = transact(realm);
                transaction.Commit();
                return returnValue;
            }
        }
    }
}
