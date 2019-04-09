using Toggl.Shared.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseTag : ITag, IDatabaseSyncable, IPotentiallyInaccessible
    {
        IDatabaseWorkspace Workspace { get; }
    }
}
