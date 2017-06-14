using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.UI;
using PropertyChanged;
using Toggl.Foundation.MvvmCross.Helper;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [ImplementPropertyChanged]
    public class OnboardingViewModel : BaseViewModel
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
        public bool IsNextVisible => nextCanExecute();

        [DependsOn(nameof(CurrentPage))]
        public bool IsPreviousVisible => previousCanExecute();

        [DependsOn(nameof(CurrentPage))]
        public MvxColor BorderColor => PageInfo[CurrentPage].BorderColor;

        [DependsOn(nameof(CurrentPage))]
        public MvxColor BackgroundColor => PageInfo[CurrentPage].BackgroundColor;

        public IMvxCommand SkipCommand { get; }

        public IMvxCommand NextCommand { get; }

        public IMvxCommand PreviousCommand { get; }

        public int NumberOfPages => PageInfo.Length;

        public OnboardingViewModel()
        {
            SkipCommand = new MvxCommand(skip);
            NextCommand = new MvxCommand(next, nextCanExecute);
            PreviousCommand = new MvxCommand(previous, previousCanExecute);
        }

        private void skip()
            => CurrentPage = LoginPage;

        private bool nextCanExecute()
            => CurrentPage < NumberOfPages - 1;

        private bool previousCanExecute()
            => CurrentPage > TrackPage;

        private void next() => CurrentPage++;

        private void previous() => CurrentPage--;
    }
}
