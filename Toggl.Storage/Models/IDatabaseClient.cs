using Toggl.Shared.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseClient : IClient, IDatabaseSyncable, IPotentiallyInaccessible
    {
        IDatabaseWorkspace Workspace { get; }
    }
}
