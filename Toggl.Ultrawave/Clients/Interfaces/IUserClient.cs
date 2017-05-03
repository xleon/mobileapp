using System;

namespace Toggl.Ultrawave.Clients
{
    public interface IUserClient
    {
        IObservable<User> Get(string username, string password);
    }
}
