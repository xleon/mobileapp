using Toggl.Shared.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabasePreferences : IPreferences, IDatabaseSyncable, IIdentifiable
    {
    }
}
