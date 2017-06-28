using UIKit;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform;
using Toggl.Foundation.MvvmCross;
using Toggl.Ultrawave.Network;
using Toggl.PrimeRadiant.Realm;
using Toggl.Ultrawave;
using Foundation;
using Toggl.Daneel.Presentation;
using Toggl.Foundation.Login;
using Toggl.Daneel.Services;

namespace Toggl.Daneel
{
    public partial class Setup : MvxIosSetup
    {
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

        protected override IMvxApplication CreateApp() => new App();

        protected override IMvxTrace CreateDebugTrace() => new DebugTrace();

        protected override void InitializeFirstChance()
        {
            base.InitializeFirstChance();

            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            var userAgent = new UserAgent("Daneel", version.ToString());

            var apiFactory = new ApiFactory(Environment, userAgent);
            var loginManager = new LoginManager(apiFactory, () => new Database());

            Mvx.RegisterSingleton<ILoginManager>(loginManager);
        }
    }
}
