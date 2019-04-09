using Toggl.Shared.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface ITasksApi
        : IPullingApiClient<ITask>,
          IPullingChangedApiClient<ITask>,
          ICreatingApiClient<ITask>
    {
    }
}
