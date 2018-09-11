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

        public void SetApiToken(string apiToken)
        {
            userDefaults.SetString(apiToken, ApiTokenKey);
            userDefaults.Synchronize();
        }

        public void SetNeedsSync(bool value)
        {
            userDefaults.SetBool(value, NeedsSyncKey);
            userDefaults.Synchronize();
        }

        public string GetApiToken() => userDefaults.StringForKey(ApiTokenKey);

        public bool GetNeedsSync() => userDefaults.BoolForKey(NeedsSyncKey);

        public void DeleteEverything() 
        {
            userDefaults.RemoveObject(ApiTokenKey);
            userDefaults.RemoveObject(NeedsSyncKey);
            userDefaults.Synchronize();
        }
    }
}
