using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.UI;
using PropertyChanged;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class OnboardingViewModel : MvxViewModel
    {
        public const int TrackPage = 0;
        public const int LogPage = 1;
        public const int SummaryPage = 2;
        public const int LoginPage = 3;

        private static readonly (MvxColor BackgroundColor, MvxColor BorderColor)[] PageInfo =
        {
            (Color.Onboarding.TrackPageBackgroundColor, Color.Onboarding.TrackPageBorderColor),
            (Color.Onboarding.LogPageBackgroundColor, Color.Onboarding.LogPageBorderColor),
            (Color.Onboarding.SummaryPageBackgroundColor, Color.Onboarding.SummaryPageBorderColor),
            (Color.Onboarding.LoginPageBackgroundColor, MvxColors.Transparent)
        };

        private readonly IMvxNavigationService navigationService;

        private int currentPage = TrackPage;
        public int CurrentPage 
        { 
            get { return currentPage; } 
            set
            {
                if (currentPage == value) return;
                if (value < 0 || value >= NumberOfPages) return;

                currentPage = value;
                RaisePropertyChanged();
                NextCommand.RaiseCanExecuteChanged();
                PreviousCommand.RaiseCanExecuteChanged();
            }
        }

        [DependsOn(nameof(CurrentPage))]
        public bool IsFirstPage => CurrentPage == 0;

        [DependsOn(nameof(CurrentPage))]
        public bool IsLastPage => CurrentPage == NumberOfPages - 1;

        [DependsOn(nameof(CurrentPage))]
        public MvxColor BorderColor => PageInfo[CurrentPage].BorderColor;

        [DependsOn(nameof(CurrentPage))]
        public MvxColor BackgroundColor => PageInfo[CurrentPage].BackgroundColor;

        public IMvxCommand SkipCommand { get; }

        public IMvxCommand NextCommand { get; }

        public IMvxCommand PreviousCommand { get; }

        public IMvxAsyncCommand LoginCommand { get; }

        public IMvxAsyncCommand SignUpCommand { get; }

        public int NumberOfPages => PageInfo.Length;

        public OnboardingViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            SkipCommand = new MvxCommand(skip);
            NextCommand = new MvxCommand(next, nextCanExecute);
            LoginCommand = new MvxAsyncCommand(login);
            SignUpCommand = new MvxAsyncCommand(signup);
            PreviousCommand = new MvxCommand(previous, previousCanExecute);
        }

        private Task login()
            => navigationService.Navigate<LoginViewModel, LoginParameter>(LoginParameter.Login);

        private Task signup()
            => navigationService.Navigate<LoginViewModel, LoginParameter>(LoginParameter.SignUp);

        private void skip()
            => CurrentPage = LoginPage;

        private bool nextCanExecute() => !IsLastPage;

        private bool previousCanExecute() => !IsFirstPage;

        private void next() => CurrentPage++;

        private void previous() => CurrentPage--;
    }
}
