using System;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients.Interfaces
{
    public interface ILocationApi
    {
        IObservable<ILocation> Get();
    }
}
