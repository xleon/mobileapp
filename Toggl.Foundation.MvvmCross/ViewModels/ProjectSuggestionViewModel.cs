using MvvmCross.Core.ViewModels;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class ProjectSuggestionViewModel : MvxNotifyPropertyChanged, ITimeEntrySuggestionViewModel
    {
        public int NumberOfTasks { get; } = 0;

        public string ProjectName { get; } = "";

        public string ClientName { get; } = "";

        public string ProjectColor { get; }

        public ProjectSuggestionViewModel(IDatabaseProject project, int numberOfTasks)
        {
            ProjectName = project.Name;
            ProjectColor = project.Color;
            NumberOfTasks = numberOfTasks;

            if (project.Client == null) return;

            ClientName = project.Client.Name;
        }
    }
}
