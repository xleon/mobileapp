using System;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IUserApi
    {
        IObservable<User> Get();
    }
}
