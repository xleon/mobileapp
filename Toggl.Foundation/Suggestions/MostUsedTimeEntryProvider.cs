using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Suggestions
{
    public class MostUsedTimeEntryProvider : ISuggestionProvider
    {
        private readonly ITogglDatabase database;
        private readonly ITimeService timeService;

        public MostUsedTimeEntryProvider(ITogglDatabase database, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.database = database;
            this.timeService = timeService;
        }

        public IObservable<Suggestion> GetSuggestions()
            => database.TimeEntries
                       .GetAll(isInLastTwoWeeks)
                       .Select(mostUsedTimeEntry)
                       .Where(isNotNull);
        
        private bool isInLastTwoWeeks(ITimeEntry timeEntry)
        {
            var twoWeeks = TimeSpan.FromDays(14);
            var delta = timeService.CurrentDateTime - timeEntry.Start;
            return delta <= twoWeeks;
        }

        private Suggestion mostUsedTimeEntry(IEnumerable<IDatabaseTimeEntry> timeEntries)
        {
            var mostUsedGroup = timeEntries.GroupBy(te => new { te.Description, te.ProjectId, te.TaskId })
                                           .OrderByDescending(g => g.Count())
                                           .FirstOrDefault();

            if (mostUsedGroup == null)
                return null;
            
            return new Suggestion(mostUsedGroup.First());
        }

        private bool isNotNull(Suggestion timeEntry)
            => timeEntry != null;
    }
}
