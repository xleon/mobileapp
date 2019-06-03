using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class PushNotificationsTokenServiceIos : IPushNotificationsTokenService
    {
        public PushNotificationsToken? Token { get; }

        public void InvalidateCurrentToken()
        {
            throw new System.NotImplementedException();
        }
    }
}
