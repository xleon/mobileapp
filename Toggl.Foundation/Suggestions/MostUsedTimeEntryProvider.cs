using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.Models;

namespace Toggl.Foundation.Suggestions
{
    public class MostUsedTimeEntryProvider : ISuggestionProvider
    {
        private readonly ITogglDatabase database;

        public MostUsedTimeEntryProvider(ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));

            this.database = database;
        }

        public IObservable<ITimeEntry> GetSuggestion()
            => database.TimeEntries
                       .GetAll(isInLastTwoWeeks)
                       .Select(mostUsedTimeEntry)
                       .Where(isNotNull);
        
        private bool isInLastTwoWeeks(ITimeEntry timeEntry)
        {
            var twoWeeks = TimeSpan.FromDays(14);
            var delta = DateTime.UtcNow - timeEntry.Start;
            return delta <= twoWeeks;
        }

        private ITimeEntry mostUsedTimeEntry(IEnumerable<ITimeEntry> timeEntries)
        {
            var mostUsedGroup = timeEntries.GroupBy(te => new { te.Description, te.ProjectId, te.TaskId })
                                           .OrderByDescending(g => g.Count())
                                           .FirstOrDefault();

            if (mostUsedGroup == null)
                return null;

            return new TimeEntry
            {
                Description = mostUsedGroup.Key.Description,
                ProjectId = mostUsedGroup.Key.ProjectId,
                TaskId = mostUsedGroup.Key.TaskId
            };
        }

        private bool isNotNull(ITimeEntry timeEntry)
            => timeEntry != null;
    }
}
