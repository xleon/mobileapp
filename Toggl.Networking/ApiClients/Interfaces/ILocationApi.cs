using System;
using Toggl.Shared.Models;

namespace Toggl.Ultrawave.ApiClients.Interfaces
{
    public interface ILocationApi
    {
        IObservable<ILocation> Get();
    }
}
