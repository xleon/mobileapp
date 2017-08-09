using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseTag : ITag, IDatabaseSyncable 
    {
        IDatabaseWorkspace Workspace { get; }
    }
}
