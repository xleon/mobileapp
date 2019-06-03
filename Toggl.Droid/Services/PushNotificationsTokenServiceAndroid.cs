using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.Droid.Services
{
    public sealed class PushNotificationsTokenServiceAndroid : IPushNotificationsTokenService
    {
        public PushNotificationsToken? Token { get; }

        public void InvalidateCurrentToken()
        {
            throw new System.NotImplementedException();
        }
    }
}
