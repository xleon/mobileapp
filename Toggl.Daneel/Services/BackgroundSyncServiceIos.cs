using System;
using Foundation;
using Toggl.Foundation.Services;
using UIKit;

namespace Toggl.Daneel.Services
{
    public sealed class BackgroundSyncServiceIos : BaseBackgroundSyncService
    {
        private const double minimumBackgroundFetchInterval = 15 * 60;

        public override void EnableBackgroundSync()
            => UIApplication.SharedApplication
                .SetMinimumBackgroundFetchInterval(minimumBackgroundFetchInterval);

        public override void DisableBackgroundSync()
            => UIApplication.SharedApplication
                .SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalNever);
    }
}
