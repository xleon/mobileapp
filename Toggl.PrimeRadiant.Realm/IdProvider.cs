using System.Linq;
using Realms;

namespace Toggl.PrimeRadiant.Realm
{
    public class RealmIdProvider : RealmObject
    {
        [PrimaryKey]
        public int Key { get; set; }

        public long Id { get; set; }
    }

    public sealed class IdProvider : IIdProvider
    {
        public long GetNextIdentifier()
        {
            var nextIdentifier = -1L;
            var realm = Realms.Realm.GetInstance();
            using (var transaction = realm.BeginWrite())
            {
                var entity = realm.All<RealmIdProvider>().SingleOrDefault();
                if (entity == null)
                {
                    entity = new RealmIdProvider { Id = -2 };
                    realm.Add(entity);
                }
                else
                {
                    nextIdentifier = entity.Id;
                    entity.Id = nextIdentifier - 1;
                }

                transaction.Commit();
            }

            return nextIdentifier;
        }
    }
}
