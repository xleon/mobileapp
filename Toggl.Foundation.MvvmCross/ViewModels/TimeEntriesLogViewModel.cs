using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Models;
using MvvmCross.Core.Navigation;
using Toggl.Foundation.MvvmCross.Parameters;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public class TimeEntriesLogViewModel : MvxViewModel
    {
        private IDisposable updateDisposable;

        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;

        public bool IsWelcome { get; set; }

        public ObservableCollection<TimeEntryViewModelCollection> TimeEntries { get; }
            = new ObservableCollection<TimeEntryViewModelCollection>();

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

        public MvxAsyncCommand<TimeEntryViewModel> EditCommand { get; }

        public TimeEntriesLogViewModel(ITogglDataSource dataSource, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.navigationService = navigationService;

            EditCommand = new MvxAsyncCommand<TimeEntryViewModel>(edit);
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            var timeEntries = await dataSource.TimeEntries.GetAll();

            timeEntries
                .Where(isNotRunning)
                .OrderByDescending(te => te.Start)
                .Select(te => new TimeEntryViewModel(te))
                .GroupBy(te => te.Start.Date)
                .Select(grouping => new TimeEntryViewModelCollection(grouping.Key, grouping))
                .ForEach(TimeEntries.Add);

            var updateObservable =
                dataSource.TimeEntries.TimeEntryUpdated
                          .Do(onTimeEntryUpdated)
                          .Where(x => !x.IsDeleted);

            updateDisposable =
                dataSource.TimeEntries.TimeEntryCreated
                    .Where(isNotRunning)
                    .Merge(updateObservable)
                    .Subscribe(onTimeEntryCreated);
        }

        private void onTimeEntryUpdated(IDatabaseTimeEntry timeEntry)
        {
            var collection = TimeEntries.SingleOrDefault(c => c.Date == timeEntry.Start.Date);
            if (collection == null) return;

            var viewModel = collection.SingleOrDefault(vm => vm.Id == timeEntry.Id);
            if (viewModel == null) return;

            collection.Remove(viewModel);
            RaisePropertyChanged(nameof(IsEmpty));
        }

        private void onTimeEntryCreated(IDatabaseTimeEntry timeEntry)
        {
            var indexDate = timeEntry.Start.Date;
            var timeEntriesInDay = new List<TimeEntryViewModel> { new TimeEntryViewModel(timeEntry) };

            var collection = TimeEntries.FirstOrDefault(x => x.Date == indexDate);
            if (collection != null)
            {
                timeEntriesInDay.AddRange(collection);
                TimeEntries.Remove(collection);
            }

            var newCollection = new TimeEntryViewModelCollection(indexDate, timeEntriesInDay.OrderBy(te => te.Start));

            var foundIndex = TimeEntries.IndexOf(TimeEntries.FirstOrDefault(x => x.Date < indexDate));
            var indexToInsert = foundIndex == -1 ? TimeEntries.Count : foundIndex;
            TimeEntries.Insert(indexToInsert, newCollection);

            RaisePropertyChanged(nameof(IsEmpty));
        }

        private bool isNotRunning(IDatabaseTimeEntry timeEntry) => timeEntry.Stop != null;

        private Task edit(TimeEntryViewModel timeEntryViewModel)
            => navigationService.Navigate<EditTimeEntryViewModel, IdParameter>(IdParameter.WithId(timeEntryViewModel.Id));
    }
}
