using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Realms;
using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal interface IRealmAdapter<TModel>
    {
        IQueryable<TModel> GetAll();

        void Delete(long id);

        TModel Create(TModel entity);

        TModel Update(long id, TModel entity);

        IEnumerable<(ConflictResolutionMode ResolutionMode, TModel Entity)> BatchUpdate(
            IEnumerable<(long Id, TModel Entity)> batch,
            Func<(long Id, TModel Entity), Expression<Func<TModel, bool>>> matchEntity,
            Func<TModel, TModel, ConflictResolutionMode> conflictResolution);
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
        
        public TModel Update(long id, TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            return doModyfingTransaction(id, (realm, realmEntity) => realmEntity.SetPropertiesFrom(entity, realm));
        }

        public IEnumerable<(ConflictResolutionMode ResolutionMode, TModel Entity)> BatchUpdate(
            IEnumerable<(long Id, TModel Entity)> batch,
            Func<(long Id, TModel Entity), Expression<Func<TModel, bool>>> matchEntity,
            Func<TModel, TModel, ConflictResolutionMode> conflictResolution)
        {
            Ensure.Argument.IsNotNull(batch, nameof(batch));
            Ensure.Argument.IsNotNull(matchEntity, nameof(matchEntity));
            Ensure.Argument.IsNotNull(conflictResolution, nameof(conflictResolution));

            var realm = Realms.Realm.GetInstance();
            using (var transaction = realm.BeginWrite())
            {
                var realmEntities = realm.All<TRealmEntity>();
                var resolvedEntities = batch.Select(updated =>
                {
                    var oldEntity = (TRealmEntity)realmEntities.SingleOrDefault(matchEntity(updated));
                    var resolveMode = conflictResolution(oldEntity, updated.Entity);
                    var resolvedEntity = resolveEntity(realm, oldEntity, updated.Entity, resolveMode);
                    return (resolveMode, (TModel)resolvedEntity);
                }).ToList();
                transaction.Commit();
                return resolvedEntities;
            }
        }

        public void Delete(long id)
        {
            doModyfingTransaction(id, (realm, realmEntity) => realm.Remove(realmEntity));
        }

        private static TModel doModyfingTransaction(long id, Action<Realms.Realm, TRealmEntity> transact)
            => doTransaction(realm =>
            {
                var realmEntity = realm.All<TRealmEntity>().Single(x => x.Id == id);
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

        private TRealmEntity resolveEntity(Realms.Realm realm, TRealmEntity old, TModel entity, ConflictResolutionMode resolveMode)
        {
            switch (resolveMode)
            {
                case ConflictResolutionMode.Create:
                    return realm.Add(convertToRealm(entity, realm));

                case ConflictResolutionMode.Delete:
                    realm.Remove(old);
                    return null;

                case ConflictResolutionMode.Update:
                    old.SetPropertiesFrom(entity, realm);
                    return old;

                case ConflictResolutionMode.Ignore:
                    return old;

                default:
                    throw new ArgumentException($"Unknown conflict resolution mode {resolveMode}");
            }
        }
    }
}
