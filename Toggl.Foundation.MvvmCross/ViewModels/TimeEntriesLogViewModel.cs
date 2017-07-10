using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntriesLogViewModel : BaseViewModel
    {
        private readonly ITogglDataSource dataSource;

        public bool IsWelcome { get; set; }

        public ObservableCollection<IGrouping<DateTime, ITimeEntry>> TimeEntries { get; }
            = new ObservableCollection<IGrouping<DateTime, ITimeEntry>>();

        [DependsOn(nameof(TimeEntries))]
        public bool IsEmpty => !TimeEntries.Any();

        [DependsOn(nameof(IsWelcome))]
        public string EmptyStateTitle
            => IsWelcome
            ? Resources.TimeEntriesLogEmptyStateWelcomeTitle
            : Resources.TimeEntriesLogEmptyStateTitle;

        [DependsOn(nameof(IsWelcome))]
        public string EmptyStateText
            => IsWelcome
            ? Resources.TimeEntriesLogEmptyStateWelcomeText
            : Resources.TimeEntriesLogEmptyStateText;

        public TimeEntriesLogViewModel(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            var timeEntries = await dataSource.TimeEntries.GetAll();

            timeEntries
                .OrderByDescending(te => te.Start)
                .GroupBy(te => te.Start.Date)
                .ForEach(TimeEntries.Add);
        }
    }
}
