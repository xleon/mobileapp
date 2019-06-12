using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Toggl.Core.Services;
using System.Collections.Immutable;
using System.Collections.Generic;
using Toggl.Core.Analytics;
using System.Linq;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.Navigation;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SuggestionsViewModel : ViewModel
    {
        private const int suggestionCount = 3;

        private readonly IInteractorFactory interactorFactory;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly ITogglDataSource dataSource;
        private readonly IRxActionFactory rxActionFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ITimeService timeService;
        private readonly IPermissionsChecker permissionsChecker;

        public IObservable<IImmutableList<Suggestion>> Suggestions { get; private set; }
        public IObservable<bool> IsEmpty { get; private set; }
        public RxAction<Suggestion, IThreadSafeTimeEntry> StartTimeEntry { get; private set; }

        public SuggestionsViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            IOnboardingStorage onboardingStorage,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            IAnalyticsService analyticsService,
            ITimeService timeService,
            IPermissionsChecker permissionsChecker,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));

            this.interactorFactory = interactorFactory;
            this.onboardingStorage = onboardingStorage;
            this.schedulerProvider = schedulerProvider;
            this.dataSource = dataSource;
            this.rxActionFactory = rxActionFactory;
            this.analyticsService = analyticsService;
            this.timeService = timeService;
            this.permissionsChecker = permissionsChecker;
        }

        public override Task Initialize()
        {
            base.Initialize();

            StartTimeEntry = rxActionFactory.FromObservable<Suggestion, IThreadSafeTimeEntry>(startTimeEntry);

            Suggestions = interactorFactory.ObserveWorkspaceOrTimeEntriesChanges().Execute()
                .StartWith(Unit.Default)
                .SelectMany(_ => getSuggestions())
                .WithLatestFrom(permissionsChecker.CalendarPermissionGranted, (suggestions, isCalendarAuthorized) => (suggestions, isCalendarAuthorized))
                .Do(item => trackPresentedSuggestions(item.suggestions, item.isCalendarAuthorized))
                .Select(item => item.suggestions)
                .AsDriver(onErrorJustReturn: ImmutableList.Create<Suggestion>(), schedulerProvider: schedulerProvider);

            IsEmpty = Suggestions
                .Select(suggestions => suggestions.None())
                .StartWith(true)
                .AsDriver(onErrorJustReturn: true, schedulerProvider: schedulerProvider);

            return base.Initialize();
        }

        private IObservable<IImmutableList<Suggestion>> getSuggestions()
            => interactorFactory.GetSuggestions(suggestionCount).Execute()
                .Select(suggestions => suggestions.ToImmutableList());

        private IObservable<IThreadSafeTimeEntry> startTimeEntry(Suggestion suggestion)
        {
            onboardingStorage.SetTimeEntryContinued();

            var timeEntry = interactorFactory
                .StartSuggestion(suggestion)
                .Execute();

            analyticsService.SuggestionStarted.Track(suggestion.ProviderType);

            if (suggestion.ProviderType == SuggestionProviderType.Calendar)
                trackCalendarOffset(suggestion);

            return timeEntry;
        }

        private void trackPresentedSuggestions(IImmutableList<Suggestion> suggestions, bool isAuthorized)
        {
            var suggestionsCount = suggestions
                .GroupBy(s => s.ProviderType)
                .Select(group => (group.Key, group.Count()));

            var workspaceCount = suggestions
                .Select(suggestion => suggestion.WorkspaceId)
                .Distinct()
                .Count();

            analyticsService.Track(new SuggestionsPresentedEvent(suggestionsCount, isAuthorized, workspaceCount));
        }

        private void trackCalendarOffset(Suggestion suggestion)
        {
            var currentTime = timeService.CurrentDateTime;
            var startTime = suggestion.StartTime;

            var offset = currentTime - startTime;

            analyticsService.Track(new CalendarSuggestionContinuedEvent(offset));
        }
    }
}
