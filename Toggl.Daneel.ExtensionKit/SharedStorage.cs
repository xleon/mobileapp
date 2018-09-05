using System;
using Foundation;

namespace Toggl.Daneel.ExtensionKit
{
    public class SharedStorage
    {
        private const string ApiTokenKey = "APITokenKey";
        private const string NeedsSyncKey = "NeedsSyncKey";

        private NSUserDefaults userDefaults;

        private SharedStorage()
        {
            var bundleId = NSBundle.MainBundle.BundleIdentifier;
            if (bundleId.Contains("SiriExtension"))
            {
                bundleId = bundleId.Substring(0, bundleId.LastIndexOf("."));
            }
            userDefaults = new NSUserDefaults($"group.{bundleId}.extensions", NSUserDefaultsType.SuiteName);
        }

        public static SharedStorage instance => new SharedStorage();

        public void setApiToken(string apiToken)
        {
            userDefaults.SetString(apiToken, ApiTokenKey);
            userDefaults.Synchronize();
        }

        public void setNeedsSync(bool value)
        {
            userDefaults.SetBool(value, NeedsSyncKey);
            userDefaults.Synchronize();
        }

        public string getApiToken() => userDefaults.StringForKey(ApiTokenKey);

        public bool getNeedsSync() => userDefaults.BoolForKey(NeedsSyncKey);
    }
}
