using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseTimeEntry : ITimeEntry, IDatabaseSyncable
    {
        IDatabaseTask Task { get; }

        IDatabaseUser User { get; }
        
        IDatabaseProject Project { get; }

        IDatabaseWorkspace Workspace { get; }
    }
}
