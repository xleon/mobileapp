using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SettingsViewModel : MvxViewModel
    {
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;

        public string Title { get; private set; }

        public IMvxAsyncCommand LogoutCommand { get; }

        public IMvxAsyncCommand BackCommand { get; }

        public SettingsViewModel(ITogglDataSource dataSource, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            Title = Resources.Settings;

            this.dataSource = dataSource;
            this.navigationService = navigationService;

            BackCommand = new MvxAsyncCommand(back);
            LogoutCommand = new MvxAsyncCommand(logout);
        }

        private Task back() => navigationService.Close(this);

        private async Task logout()
        {
            await dataSource.Logout();
            await navigationService.Navigate<OnboardingViewModel>();
        }
    }
}
