using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.Core.UI.ViewModels.MainLog.Identity;
using Toggl.Shared;


namespace Toggl.Core.UI.Extensions
{
    using MainLogSection = AnimatableSectionModel<MainLogSectionViewModel, MainLogItemViewModel, IMainLogKey>;

    public static class MainLogSectionExtensions
    {
        public static IObservable<IImmutableList<MainLogSection>> MergeToMainLogSections(
            this IObservable<IImmutableList<MainLogSection>> timeEntries,
            IObservable<IImmutableList<Suggestion>> suggestions,
            IObservable<bool> shouldShowRatingView,
            MainLogSection userFeedbackMainLogSection
        )
            => timeEntries
                .CombineLatest(suggestions, shouldShowRatingView, (te, s, r) => mergeMainLogItems(te, s, r, userFeedbackMainLogSection));

        private static IImmutableList<MainLogSection> mergeMainLogItems(
            IImmutableList<MainLogSection> timeEntries,
            IImmutableList<Suggestion> suggestions,
            bool shouldShowRatingView,
            MainLogSection userFeedbackMainLogSection)
        {
            if (suggestions.Count <= 0)
            {
                return shouldShowRatingView
                    ? timeEntries.Prepend(userFeedbackMainLogSection).ToImmutableList()
                    : timeEntries.ToImmutableList();
            }

            var suggestionList = suggestions.Select(suggestionToMainLogItem);
            var suggestionsHeaderViewModel = new SuggestionsHeaderViewModel(suggestions.Count > 1 ? Resources.WorkingOnThese : Resources.WorkingOnThis);
            var suggestionsSection = new MainLogSection(suggestionsHeaderViewModel, suggestionList);
            return shouldShowRatingView
                ? timeEntries.Prepend(userFeedbackMainLogSection).Prepend(suggestionsSection).ToImmutableList()
                : timeEntries.Prepend(suggestionsSection).ToImmutableList();
        }

        private static MainLogItemViewModel suggestionToMainLogItem(Suggestion suggestion, int position)
            => new SuggestionLogItemViewModel(position, suggestion);
    }
}
