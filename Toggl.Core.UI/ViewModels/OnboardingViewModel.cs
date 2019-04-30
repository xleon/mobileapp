using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Extensions;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Math = System.Math;
using Colors = Toggl.Core.UI.Helper.Colors;
using Toggl.Core.UI.Parameters;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class OnboardingViewModel : ViewModel
    {
        public const int TrackPage = 0;
        public const int MostUsedPage = 1;
        public const int ReportsPage = 2;

        private static readonly string[] pageNames =
        {
            nameof(TrackPage),
            nameof(MostUsedPage),
            nameof(ReportsPage)
        };

        private static readonly (Color BackgroundColor, Color BorderColor)[] pageInfo =
        {
            (Colors.Onboarding.TrackPageBackgroundColor, Colors.Onboarding.TrackPageBorderColor),
            (Colors.Onboarding.LogPageBackgroundColor, Colors.Onboarding.LogPageBorderColor),
            (Colors.Onboarding.ReportsPageBackgroundColor, Colors.Onboarding.ReportsPageBorderColor)
        };

        private readonly IAnalyticsService analyticsService;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly INavigationService navigationService;
        private readonly BehaviorSubject<int> currentPage = new BehaviorSubject<int>(0);
        private readonly bool[] pagesVisited = new bool[pageInfo.Length];
        private bool visitedAllPages => pagesVisited.All(CommonFunctions.Identity);

        public IObservable<int> CurrentPage { get; }
        public IObservable<bool> IsTrackPage { get; }
        public IObservable<bool> IsReportPage { get; }
        public IObservable<bool> IsSummaryPage { get; }
        public IObservable<bool> IsFirstPage { get; }
        public IObservable<bool> IsLastPage { get; }
        public IObservable<Color> BorderColor { get; }
        public IObservable<Color> BackgroundColor { get; }

        public UIAction SkipOnboarding { get; }
        public UIAction GoToNextPage { get; }
        public UIAction GoToPreviousPage { get; }

        public int NumberOfPages => pageInfo.Length;

        public OnboardingViewModel(
            INavigationService navigationService,
            IOnboardingStorage onboardingStorage,
            IAnalyticsService analyticsService,
            IRxActionFactory rxActionFactory,
            ISchedulerProvider schedulerProvider)
        {
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.analyticsService = analyticsService;
            this.navigationService = navigationService;
            this.onboardingStorage = onboardingStorage;

            pagesVisited[0] = true;

            CurrentPage = currentPage.AsDriver(schedulerProvider);
            IsTrackPage = currentPage.Select(p => p == TrackPage).DistinctUntilChanged().AsDriver(schedulerProvider);
            IsReportPage = currentPage.Select(p => p == ReportsPage).DistinctUntilChanged().AsDriver(schedulerProvider);
            IsSummaryPage = currentPage.Select(p => p == MostUsedPage).DistinctUntilChanged().AsDriver(schedulerProvider);

            var isFirstPage = currentPage.Select(p => p == TrackPage).DistinctUntilChanged();
            IsFirstPage = isFirstPage.AsDriver(schedulerProvider);
            IsLastPage = currentPage.Select(p => p == pageInfo.Length - 1).DistinctUntilChanged().AsDriver(schedulerProvider);
            BorderColor = currentPage.Select(p => pageInfo[p].BorderColor).AsDriver(schedulerProvider);
            BackgroundColor = currentPage.Select(p => pageInfo[p].BackgroundColor).AsDriver(schedulerProvider);

            SkipOnboarding = rxActionFactory.FromObservable(skip);
            GoToNextPage = rxActionFactory.FromAsync(next);
            GoToPreviousPage = rxActionFactory.FromAsync(previous, isFirstPage.Invert());
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            if (onboardingStorage.CompletedOnboarding())
            {
                await navigationService.Navigate<LoginViewModel, CredentialsParameter>(CredentialsParameter.Empty);
            }
        }

        public async Task ChangePage(int page)
        {
            var boundCheckedPage = Math.Max(page, 0);
            boundCheckedPage = Math.Min(boundCheckedPage, pageInfo.Length - 1);
            pagesVisited[boundCheckedPage] = true;

            if (page == pageInfo.Length)
            {
                await completeOnboarding();
                return;
            }

            currentPage.OnNext(boundCheckedPage);
        }

        private IObservable<Unit> skip() => navigationService.Navigate<LoginViewModel, CredentialsParameter>(CredentialsParameter.Empty)
            .ToObservable()
            .Do(_ => analyticsService.OnboardingSkip.Track(pageNames[currentPage.Value]));


        private Task completeOnboarding()
        {
            if (visitedAllPages)
            {
                onboardingStorage.SetCompletedOnboarding();
            }

            return navigationService.Navigate<LoginViewModel, CredentialsParameter>(CredentialsParameter.Empty);
        }

        private Task next() => ChangePage(currentPage.Value + 1);

        private Task previous() => ChangePage(currentPage.Value - 1);
    }
}
