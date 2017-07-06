using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntriesLogViewModel : BaseViewModel
    {
        private readonly ITogglDataSource dataSource;

        public ObservableCollection<IGrouping<DateTime, ITimeEntry>> TimeEntries { get; }
            = new ObservableCollection<IGrouping<DateTime, ITimeEntry>>();

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
