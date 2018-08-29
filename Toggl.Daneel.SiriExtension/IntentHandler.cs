using System;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;
using Toggl.Daneel.Intents;
using Foundation;
using Intents;

namespace SiriExtension
{
    [Register("IntentHandler")]
    public class IntentHandler : INExtension
    {
        #if USE_PRODUCTION_API
        private const ApiEnvironment environment = ApiEnvironment.Production;
        #else
        private const ApiEnvironment environment = ApiEnvironment.Staging;
        #endif

        protected IntentHandler(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override NSObject GetHandler(INIntent intent)
        {
            var bundleId = NSBundle.MainBundle.BundleIdentifier;
            bundleId = bundleId.Substring(0, bundleId.LastIndexOf("."));
            var userDefaults = new NSUserDefaults($"group.{bundleId}.extensions", NSUserDefaultsType.SuiteName);

            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
            var apiToken = userDefaults.StringForKey("api-token");
            var userAgent = new UserAgent("Daneel", $"{version}.SiriExtension");
            var togglApi = new TogglApi(new ApiConfiguration(environment, Credentials.WithApiToken(apiToken), userAgent));

            if (intent is StopTimerIntent)
            {
                return new StopTimerIntentHandler();
            }
            throw new Exception("Unhandled intent type: ${intent}");
        }
    }
}
