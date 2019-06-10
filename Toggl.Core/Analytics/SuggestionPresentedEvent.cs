using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Suggestions;
using Toggl.Shared.Extensions;
using static Toggl.Core.Analytics.CalendarSuggestionProviderState;

namespace Toggl.Core.Analytics
{
    public class SuggestionPresentedEvent : ITrackableEvent
    {
        public const string SuggestionsCountName = "SuggestionsCount";
        public const string CalendarProviderStateName = "CalendarProviderState";
        
        public string EventName => "SuggestionPresented";

        private Dictionary<string, string> parameters;

        public SuggestionPresentedEvent(IEnumerable<(SuggestionProviderType Type, int Count)> suggestions, bool isCalendarAuthorized)
        {
            var counts = suggestions.ToDictionary(s => s.Type, s => s.Count);

            var providers = Enum.GetValues(typeof(SuggestionProviderType))
                .OfType<SuggestionProviderType>()
                .ToList();

            providers
                .Where(provider => !counts.ContainsKey(provider))
                .ForEach(provider => counts[provider] = 0);

            parameters = providers.ToDictionary(type => type.ToString(), type => counts[type].ToString());

            var suggestionsCount = counts.Values.Sum();
            parameters[SuggestionsCountName] = suggestionsCount.ToString();

            var calendarState = Unauthorized;
            if (isCalendarAuthorized)
            {
                calendarState = counts[SuggestionProviderType.Calendar] > 0
                    ? SuggestionsAvailable
                    : NoEvents;
            }
            parameters[CalendarProviderStateName] = calendarState.ToString();
        }

        public Dictionary<string, string> ToDictionary() => parameters;
    }
}
