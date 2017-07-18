using Foundation;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using Toggl.Daneel.Presentation;
using Toggl.Daneel.Services;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Suggestions;
using Toggl.PrimeRadiant.Realm;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;
using UIKit;

namespace Toggl.Daneel
{
    public partial class Setup : MvxIosSetup
    {
        private IMvxNavigationService navigationService;
#if DEBUG
        private const ApiEnvironment Environment = ApiEnvironment.Staging;
#else
        private const ApiEnvironment Environment = ApiEnvironment.Production;
#endif

        public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : this(applicationDelegate, new TogglPresenter(applicationDelegate, window))
        {
        }

        public Setup(MvxApplicationDelegate applicationDelegate, IMvxIosViewPresenter presenter)
            : base(applicationDelegate, presenter)
        {
        }

        protected override IMvxNavigationService InitializeNavigationService()
        {
            navigationService = base.InitializeNavigationService();
            return navigationService;
        }

        protected override IMvxApplication CreateApp()
        {
            base.InitializeFirstChance();

            var database = new Database();
            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            var userAgent = new UserAgent("Daneel", version.ToString());

            var apiFactory = new ApiFactory(Environment, userAgent);
            var loginManager = new LoginManager(apiFactory, database);

            Mvx.RegisterSingleton<ISuggestionProviderContainer>(
                new SuggestionProviderContainer(
                    new MostUsedTimeEntryProvider(database)
                )
            );

            return new App(loginManager, navigationService);
        }

        protected override IMvxTrace CreateDebugTrace() => new DebugTrace();

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.RegisterSingleton<IPasswordManagerService>(new OnePasswordService());
        }
    }
}
