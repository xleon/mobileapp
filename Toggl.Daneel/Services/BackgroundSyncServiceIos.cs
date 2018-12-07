using System;
using Foundation;
using Toggl.Foundation.Services;
using UIKit;

namespace Toggl.Daneel.Services
{
    public sealed class BackgroundSyncServiceIos : BaseBackgroundSyncService
    {
        private const double minimumBackgroundFetchIntervalInMinutes = 15;

        public override void EnableBackgroundSync()
            => UIApplication.SharedApplication
                .SetMinimumBackgroundFetchInterval(TimeSpan.FromMinutes(minimumBackgroundFetchIntervalInMinutes).TotalSeconds);

        public override void DisableBackgroundSync()
            => UIApplication.SharedApplication
                .SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalNever);
    }
}
