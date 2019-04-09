using System;
using Toggl.Shared.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IPreferencesApi
        : IUpdatingApiClient<IPreferences>,
          IPullingSingleApiClient<IPreferences>
    {
    }
}
