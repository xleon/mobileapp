using Toggl.Foundation.Sync;

namespace Toggl.Foundation.Extensions
{
    public static class SyncManagerExtensions
    {
        public static void InitiatePushSync(this ISyncManager syncManager)
        {
            var _ = syncManager.PushSync();
        }
    }
}
