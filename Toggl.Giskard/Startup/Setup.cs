using System.Reactive.Concurrency;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.Plugins;
using Android.Content;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross;
using Toggl.Giskard.Services;
using Toggl.PrimeRadiant.Realm;
using Toggl.Ultrawave;

namespace Toggl.Giskard
{
    public sealed class Setup : MvxAndroidSetup
    {
        private IMvxNavigationService navigationService;

#if USE_PRODUCTION_API
        private const ApiEnvironment environment = ApiEnvironment.Production;
#else
        private const ApiEnvironment environment = ApiEnvironment.Staging;
#endif

        public Setup(Context applicationContext) 
            : base(applicationContext)
        {
        }

        protected override IMvxTrace CreateDebugTrace() => new DebugTrace();

        protected override IMvxApplication CreateApp() => new App();

        protected override IMvxNavigationService InitializeNavigationService(IMvxViewModelLocatorCollection collection)
            => navigationService = base.InitializeNavigationService(collection);

        protected override void InitializeApp(IMvxPluginManager pluginManager, IMvxApplication app)
        {
            base.InitializeApp(pluginManager, app);

            const string clientName = "Giskard";
            var packageInfo = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0);
            var version = packageInfo.VersionName;

            var sharedPreferences = ApplicationContext.GetSharedPreferences(clientName, FileCreationMode.Private);

            var foundation = Foundation.Foundation.Create(
                clientName,
                version,
                new Database(),
                new TimeService(Scheduler.Default),
                new GoogleService(),
                environment
            );

            foundation.RegisterServices(new DialogService(), new BrowserService(), 
                                        new SharedPreferencesStorage(sharedPreferences),
                                        navigationService, new OnePasswordService())
                      .RevokeNewUserIfNeeded()
                      .Initialize(app as App, Scheduler.Default);
        }
    }
}
