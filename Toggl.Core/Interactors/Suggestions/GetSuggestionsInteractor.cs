using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Suggestions;
using Toggl.Shared;
using Toggl.Core.Diagnostics;

namespace Toggl.Core.Interactors.Suggestions
{
    public sealed class GetSuggestionsInteractor : IInteractor<IObservable<IEnumerable<Suggestion>>>
    {
        private readonly int suggestionCount;
        private readonly IStopwatchProvider stopwatchProvider;
        private readonly ITogglDataSource dataSource;
        private readonly ITimeService timeService;

        public GetSuggestionsInteractor(
            int suggestionCount,
            IStopwatchProvider stopwatchProvider,
            ITogglDataSource dataSource,
            ITimeService timeService)
        {
            Ensure.Argument.IsInClosedRange(suggestionCount, 1, 9, nameof(suggestionCount));
            Ensure.Argument.IsNotNull(stopwatchProvider, nameof(stopwatchProvider));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.stopwatchProvider = stopwatchProvider;
            this.dataSource = dataSource;
            this.timeService = timeService;
            this.suggestionCount = suggestionCount;
        }

        public IObservable<IEnumerable<Suggestion>> Execute()
            => getSuggestionProviders()
                .Select(provider => provider.GetSuggestions())
                .Aggregate(Observable.Concat)
                .Take(suggestionCount)
                .ToList();

        private IReadOnlyList<ISuggestionProvider> getSuggestionProviders()
        {
            return new List<ISuggestionProvider>
            {
                new RandomForestSuggestionProvider(stopwatchProvider, dataSource, timeService, suggestionCount),
                new MostUsedTimeEntrySuggestionProvider(timeService, dataSource, suggestionCount)
            };
        }
    }
}
