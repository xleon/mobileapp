using Toggl.Shared.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IClientsApi
        : IPullingApiClient<IClient>,
          IPullingChangedApiClient<IClient>,
          ICreatingApiClient<IClient>
    {
    }
}
