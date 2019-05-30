using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Suggestions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Toggl.Core.Services;
using System.Collections.Immutable;

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

        public IObservable<IImmutableList<Suggestion>> Suggestions { get; private set; }
        public IObservable<bool> IsEmpty { get; private set; }
        public InputAction<Suggestion> StartTimeEntry { get; private set; }

        public SuggestionsViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            IOnboardingStorage onboardingStorage,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;
            this.onboardingStorage = onboardingStorage;
            this.schedulerProvider = schedulerProvider;
            this.dataSource = dataSource;
            this.rxActionFactory = rxActionFactory;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            StartTimeEntry = rxActionFactory.FromAsync<Suggestion>(suggestion => startTimeEntry(suggestion));

            Suggestions = interactorFactory.ObserveWorkspaceOrTimeEntriesChanges().Execute()
                .StartWith(Unit.Default)
                .SelectMany(_ => getSuggestions())
                .AsDriver(onErrorJustReturn: ImmutableList.Create<Suggestion>(), schedulerProvider: schedulerProvider);

            IsEmpty = Suggestions
                .Select(suggestions => suggestions.None())
                .StartWith(true)
                .AsDriver(onErrorJustReturn: true, schedulerProvider: schedulerProvider);
        }

        private IObservable<IImmutableList<Suggestion>> getSuggestions()
            => interactorFactory.GetSuggestions(suggestionCount).Execute()
                .Select(suggestions => suggestions.ToImmutableList());

        private async Task startTimeEntry(Suggestion suggestion)
        {
            onboardingStorage.SetTimeEntryContinued();

            await interactorFactory
                .StartSuggestion(suggestion)
                .Execute();
        }
    }
}
