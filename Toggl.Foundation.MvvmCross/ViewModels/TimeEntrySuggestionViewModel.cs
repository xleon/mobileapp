using MvvmCross.Core.ViewModels;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class TimeEntrySuggestionViewModel : MvxNotifyPropertyChanged, ITimeEntrySuggestionViewModel
    {
        public long Id { get; }

        public string Description { get; } = "";

        public bool HasProject { get; } = false;

        public string ProjectName { get; } = "";
        
        public string ProjectColor { get; } = "";

        public string ClientName { get; } = "";

        public TimeEntrySuggestionViewModel(IDatabaseTimeEntry timeEntry)
        {
            Id = timeEntry.Id;
            Description = timeEntry.Description;

            if (timeEntry.Project == null) return;
            HasProject = true;
            ProjectName = timeEntry.Project.Name;
            ProjectColor = timeEntry.Project.Color;

            if (timeEntry.Project.Client == null) return;
            ClientName = timeEntry.Project.Client.Name;
        }
    }
}
