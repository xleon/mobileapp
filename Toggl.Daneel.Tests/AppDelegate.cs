using System.Reflection;
using Foundation;
using UIKit;

using Xunit.Runner;
using Xunit.Runners.UI;
using Xunit.Sdk;


namespace Toggl.Daneel.Tests
{
    [Register("AppDelegate")]
    public partial class AppDelegate : RunnerAppDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            // We need this to ensure the execution assembly is part of the app bundlee
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            // Tests can be inside the main assembly
            AddTestAssembly(Assembly.GetExecutingAssembly());

            // you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
            Writer = new TcpTextWriter("10.0.1.2", 16384);
            // start running the test suites as soon as the application is loaded
            AutoStart = true;
            // crash the application (to ensure it's ended) and return to springboard
            TerminateAfterExecution = true;

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}