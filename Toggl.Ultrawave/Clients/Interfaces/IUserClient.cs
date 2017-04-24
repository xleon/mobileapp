using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Clients
{
    public interface IUserClient
    {
        ICall<User> Get(string username, string password);
    }
}
