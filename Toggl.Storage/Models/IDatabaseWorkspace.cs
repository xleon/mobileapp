using Toggl.Shared.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseWorkspace : IWorkspace, IDatabaseSyncable, IPotentiallyInaccessible { }
}
