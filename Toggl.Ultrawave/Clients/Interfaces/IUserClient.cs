using System;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Clients
{
    public interface IUserClient
    {
        IObservable<User> Get();

        IObservable<User> Get(Credentials credentials);
    }
}
