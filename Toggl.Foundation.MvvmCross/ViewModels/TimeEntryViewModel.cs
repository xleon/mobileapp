using System;
using MvvmCross.Core.ViewModels;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntryViewModel : MvxNotifyPropertyChanged
    {
        private readonly IDisposable timeDisposable;

        public string Description { get; } = "";

        public string ProjectName { get; } = "";

        public string TaskName { get; } = "";

        public DateTimeOffset Start { get; }

        public TimeSpan Duration { get; private set; }

        public string ProjectColor { get; } = "#00000000";

        public bool HasProject { get; }

        public TimeEntryViewModel(IDatabaseTimeEntry timeEntry, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(timeEntry, nameof(timeEntry));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            Start = timeEntry.Start;
            Description = timeEntry.Description;
            HasProject = timeEntry.Project != null;
            Duration = (timeEntry.Stop ?? timeService.CurrentDateTime) - Start;

            if (HasProject)
            {
                ProjectName = timeEntry.Project.Name;
                ProjectColor = timeEntry.Project.Color;
            }

            if (timeEntry.Stop != null) return;
            timeDisposable = timeService.CurrentDateTimeObservable.Subscribe(currentTime => Duration = currentTime - Start);
        }
    }
}
