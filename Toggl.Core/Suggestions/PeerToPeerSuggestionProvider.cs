using System;
using System.Reactive.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.Interactors;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Suggestions
{
    public class PeerToPeerSuggestionProvider : ISuggestionProvider
    {
        private const int maxSuggestionCount = 1;

        private readonly IInteractorFactory interactorFactory;
        private readonly IObservable<(string, DateTimeOffset)> shared;

        public PeerToPeerSuggestionProvider(IInteractorFactory interactorFactory, IObservable<(string, DateTimeOffset)> shared)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.interactorFactory = interactorFactory;
            this.shared = shared;
        }

        public IObservable<Suggestion> GetSuggestions()
        {
            return interactorFactory.GetDefaultWorkspace().Execute()
                .CombineLatest(
                    shared,
                    (workspace, tuple) => suggestionFromEvent(tuple.Item1, tuple.Item2, workspace.Id))
                .Take(maxSuggestionCount)
                .OnErrorResumeEmpty();
        }

        private Suggestion suggestionFromEvent(string description, DateTimeOffset start, long workspaceId)
            => new Suggestion(description, start, workspaceId, SuggestionProviderType.PeerToPeer);

    }
}
