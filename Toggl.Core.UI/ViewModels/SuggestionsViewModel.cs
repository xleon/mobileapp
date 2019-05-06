using System;
using System.Linq;
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

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SuggestionsViewModel : ViewModel
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly ISuggestionProviderContainer suggestionProviders;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly ITogglDataSource dataSource;
        private readonly IRxActionFactory rxActionFactory;

        public IObservable<Suggestion[]> Suggestions { get; private set; }
        public IObservable<bool> IsEmpty { get; private set; }
        public RxAction<Suggestion, IThreadSafeTimeEntry> StartTimeEntry { get; private set; }

        public SuggestionsViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            IOnboardingStorage onboardingStorage,
            ISuggestionProviderContainer suggestionProviders,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(suggestionProviders, nameof(suggestionProviders));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;
            this.onboardingStorage = onboardingStorage;
            this.suggestionProviders = suggestionProviders;
            this.schedulerProvider = schedulerProvider;
            this.dataSource = dataSource;
            this.rxActionFactory = rxActionFactory;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            StartTimeEntry = rxActionFactory.FromObservable<Suggestion, IThreadSafeTimeEntry>(startTimeEntry);

            Suggestions = interactorFactory.ObserveWorkspaceOrTimeEntriesChanges().Execute()
                .StartWith(Unit.Default)
                .SelectMany(_ => getSuggestions())
                .AsDriver(onErrorJustReturn: new Suggestion[0], schedulerProvider: schedulerProvider);

            IsEmpty = Suggestions
                .Select(suggestions => suggestions.Length == 0)
                .StartWith(true)
                .AsDriver(onErrorJustReturn: true, schedulerProvider: schedulerProvider);
        }

        private IObservable<Suggestion[]> getSuggestions()
        {
            return suggestionProviders
                .Providers
                .Select(provider => provider.GetSuggestions())
                .Aggregate(Observable.Merge)
                .ToArray();
        }

        private IObservable<IThreadSafeTimeEntry> startTimeEntry(Suggestion suggestion)
        {
            onboardingStorage.SetTimeEntryContinued();

            return interactorFactory
                .StartSuggestion(suggestion)
                .Execute();
        }
    }
}
