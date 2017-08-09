using Toggl.Multivac.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseTask : ITask, IDatabaseSyncable 
    {
        IDatabaseUser User { get; }
        
        IDatabaseProject Project { get; }

        IDatabaseWorkspace Workspace { get; }
    }
}
