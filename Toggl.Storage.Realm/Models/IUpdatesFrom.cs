
namespace Toggl.PrimeRadiant.Realm
{
    internal interface IUpdatesFrom<TEntity>
    {
        void SetPropertiesFrom(TEntity entity, Realms.Realm realm);
    }
}
