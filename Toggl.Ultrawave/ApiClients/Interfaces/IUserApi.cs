using System;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.ApiClients
{
    public interface IUserApi
    {
        IObservable<User> Get();

        IObservable<User> Get(Credentials credentials);
    }
}
