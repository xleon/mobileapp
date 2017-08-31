using System.Collections.Generic;
using System.Linq;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions
{
    public sealed class TimeEntrySuggestionViewModel : BaseTimeEntrySuggestionViewModel
    {
        internal static IEnumerable<TimeEntrySuggestionViewModel> FromTimeEntries(
            IEnumerable<IDatabaseTimeEntry> timeEntries
        ) => timeEntries.Select(te => new TimeEntrySuggestionViewModel(te));

        public string Description { get; } = "";

        public bool HasProject { get; } = false;

        public long? ProjectId { get; }

        public string ProjectName { get; } = "";
        
        public string ProjectColor { get; } = "";

        public string ClientName { get; } = "";

        public TimeEntrySuggestionViewModel(IDatabaseTimeEntry timeEntry)
        {
            Description = timeEntry.Description;

            if (timeEntry.Project == null) return;
            HasProject = true;
            ProjectId = timeEntry.Project.Id;
            ProjectName = timeEntry.Project.Name;
            ProjectColor = timeEntry.Project.Color;

            if (timeEntry.Project.Client == null) return;
            ClientName = timeEntry.Project.Client.Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Description.GetHashCode() * 397) ^
                       (ProjectName.GetHashCode() * 397) ^
                       (ProjectColor.GetHashCode() * 397) ^
                       ClientName.GetHashCode();
            }
        }
    }
}
