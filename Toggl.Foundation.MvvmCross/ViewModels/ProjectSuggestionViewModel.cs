using MvvmCross.Core.ViewModels;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class ProjectSuggestionViewModel : MvxNotifyPropertyChanged, ITimeEntrySuggestionViewModel
    {
        public long Id { get; }

        public int NumberOfTasks { get; } = 0;

        public string ProjectName { get; } = "";

        public string ClientName { get; } = "";

        public string ProjectColor { get; }

        public ProjectSuggestionViewModel(IDatabaseProject project)
        {
            Id = project.Id;
            ProjectName = project.Name;
            ProjectColor = project.Color;

            if (project.Client == null) return;

            ClientName = project.Client.Name;
        }
    }
}
