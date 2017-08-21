using System;
using MvvmCross.Core.ViewModels;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntryViewModel : MvxNotifyPropertyChanged
    {
        public long Id { get; }

        public string Description { get; } = "";

        public string ProjectName { get; } = "";

        public string TaskName { get; } = "";

        public DateTimeOffset Start { get; }

        public TimeSpan Duration { get; }

        public string ProjectColor { get; } = "#00000000";

        public bool HasProject { get; }

        public TimeEntryViewModel(IDatabaseTimeEntry timeEntry)
        {
            Ensure.Argument.IsNotNull(timeEntry, nameof(timeEntry));

            Id = timeEntry.Id;
            Start = timeEntry.Start;
            Description = timeEntry.Description;
            HasProject = timeEntry.Project != null;
            Duration = timeEntry.Stop.Value - Start;

            if (HasProject)
            {
                ProjectName = timeEntry.Project.Name;
                ProjectColor = timeEntry.Project.Color;
            }
        }
    }
}
