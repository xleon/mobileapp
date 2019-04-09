using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace Toggl.Core.Login
{
    public interface IApiFactory
    {
        ITogglApi CreateApiWith(Credentials credentials);
    }
}
