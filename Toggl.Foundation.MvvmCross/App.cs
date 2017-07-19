using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross
{
    public class App : MvxApplication
    {
        private readonly AppStart appStart;

        public App(ILoginManager loginManager, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            appStart = new AppStart(loginManager, navigationService);
        }

        public override void Initialize()
        {
            Mvx.RegisterSingleton<IPasswordManagerService>(new StubPasswordManagerService());

            RegisterAppStart(appStart);
        }
    }

    public class AppStart : IMvxAppStart
    {
        private readonly ILoginManager loginManager;
        private readonly IMvxNavigationService navigationService;

        public AppStart(ILoginManager loginManager, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.loginManager = loginManager;
            this.navigationService = navigationService;
        }

        public void Start(object hint = null)
        {
            var dataSource = loginManager.GetDataSourceIfLoggedIn();
            if (dataSource == null)
            {
                Mvx.RegisterSingleton(loginManager);
                navigationService.Navigate<OnboardingViewModel>();
            }
            else
            {
                Mvx.RegisterSingleton(dataSource);
                navigationService.Navigate<MainViewModel>();
            }
        }
    }
}
