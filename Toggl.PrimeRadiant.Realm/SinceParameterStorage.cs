using System;
using System.Linq;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;
using Toggl.PrimeRadiant.Realm.Models;

namespace Toggl.PrimeRadiant.Realm
{
    public sealed class SinceParameterStorage : ISinceParameterRepository
    {
        private readonly Func<Realms.Realm> getRealmInstance;

        public SinceParameterStorage(Func<Realms.Realm> getRealmInstance)
        {
            Ensure.Argument.IsNotNull(getRealmInstance, nameof(getRealmInstance));

            this.getRealmInstance = getRealmInstance;
        }

        public ISinceParameters Get()
        {
            return doTransaction();
        }

        public void Set(ISinceParameters parameters)
        {
            doTransaction(p => p.SetValuesFrom(parameters));
        }

        private RealmSinceParameters doTransaction(Action<RealmSinceParameters> mutateParameters = null)
        {
            RealmSinceParameters parameters;

            var realm = getRealmInstance();
            using (var transaction = realm.BeginWrite())
            {
                parameters = getOrCreateRealmObject(realm);
                mutateParameters?.Invoke(parameters);
                transaction.Commit();
            }

            return parameters;
        }

        private RealmSinceParameters getOrCreateRealmObject(Realms.Realm realm)
        {
            var parameters = realm.All<RealmSinceParameters>().SingleOrDefault();
            if (parameters == null)
            {
                parameters = new RealmSinceParameters();
                realm.Add(parameters);
            }
            return parameters;
        }
    }
}
