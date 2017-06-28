using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class TimeEntriesViewModel : BaseViewModel
    {
        private readonly ITogglDataSource dataSource;

        public ObservableCollection<ITimeEntry> TimeEntries { get; }
            = new ObservableCollection<ITimeEntry>();

        public TimeEntriesViewModel(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public async override Task Initialize()
        {
            await base.Initialize();
            dataSource.TimeEntries.GetAll().Subscribe(timeEntries => timeEntries.ForEach(TimeEntries.Add));
        }
    }
}
