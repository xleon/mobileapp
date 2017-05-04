using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Repositories;

namespace Toggl.PrimeRadiant
{
    public interface ITogglDatabase
    {
        IRepository<IUser> User { get; }

        IRepository<IClient> Clients { get; }

        IRepository<IProject> Projects { get; }

        IRepository<ITag> Tags { get; }

        IRepository<ITask> Tasks { get; }

        IRepository<ITimeEntry> TimeEntries { get; }

        IRepository<IWorkspace> Workspaces { get; }
    }
}
