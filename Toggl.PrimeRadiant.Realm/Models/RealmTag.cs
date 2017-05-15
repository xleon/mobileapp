using Realms;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmTag : RealmObject, IDatabaseTag
    {
        public int WorkspaceId { get; set; }

        public string Name { get; set; }
    }
}
