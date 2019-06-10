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
        private readonly IPermissionsService permissionsService;

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
            IPermissionsService permissionsService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));

            this.interactorFactory = interactorFactory;
            this.onboardingStorage = onboardingStorage;
            this.schedulerProvider = schedulerProvider;
            this.dataSource = dataSource;
            this.rxActionFactory = rxActionFactory;
            this.analyticsService = analyticsService;
            this.timeService = timeService;
            this.permissionsService = permissionsService;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            StartTimeEntry = rxActionFactory.FromObservable<Suggestion, IThreadSafeTimeEntry>(startTimeEntry);

            Suggestions = interactorFactory.ObserveWorkspaceOrTimeEntriesChanges().Execute()
                .StartWith(Unit.Default)
                .SelectMany(_ => getSuggestions())
                .WithLatestFrom(permissionsService.CalendarPermissionGranted, (suggestions, isCalendarAuthorized) => (suggestions, isCalendarAuthorized))
                .Do(item => trackPresentedSuggestions(item.suggestions, item.isCalendarAuthorized))
                .Select(item => item.suggestions)
                .AsDriver(onErrorJustReturn: ImmutableList.Create<Suggestion>(), schedulerProvider: schedulerProvider);

            IsEmpty = Suggestions
                .Select(suggestions => suggestions.None())
                .StartWith(true)
                .AsDriver(onErrorJustReturn: true, schedulerProvider: schedulerProvider);
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

            analyticsService.Track(new SuggestionPresentedEvent(suggestionsCount, isAuthorized));
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
