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
        public override void Initialize()
        {
            
        }

        public void Initialize(ILoginManager loginManager, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            Mvx.RegisterSingleton<IPasswordManagerService>(new StubPasswordManagerService());

            RegisterAppStart(new AppStart(loginManager, navigationService));
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
            Mvx.RegisterSingleton(loginManager);

            var dataSource = loginManager.GetDataSourceIfLoggedIn();
            if (dataSource == null)
            {
                navigationService.Navigate<OnboardingViewModel>();
                return;
            }

            Mvx.RegisterSingleton(dataSource);
            navigationService.Navigate<MainViewModel>();
        }
    }
}
