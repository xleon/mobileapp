using System;
using Toggl.Foundation.Services;
using Foundation;

namespace Toggl.Daneel.Services
{
    public class PrivateSharedStorageService : IPrivateSharedStorageService
    {
        NSUserDefaults userDefaults;

        public PrivateSharedStorageService()
        {
            var bundleId = NSBundle.MainBundle.BundleIdentifier;
            userDefaults = new NSUserDefaults($"group.{bundleId}.extensions", NSUserDefaultsType.SuiteName);
        }

        public void SaveApiToken(string apiToken)
        {
            userDefaults.SetString(apiToken, "api-token");
        }
    }
}
