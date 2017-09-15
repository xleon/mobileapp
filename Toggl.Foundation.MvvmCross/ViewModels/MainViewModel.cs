using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.Sync;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class MainViewModel : MvxViewModel
    {
        private bool isWelcome = false;
        private CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;

        public TimeSpan CurrentTimeEntryElapsedTime { get; private set; } = TimeSpan.Zero;

        public ITimeEntry CurrentlyRunningTimeEntry { get; private set; }

        public bool IsSyncing { get; private set; }

        public bool SpiderIsVisible { get; set; } = true;

        public IMvxAsyncCommand StartTimeEntryCommand { get; }

        public IMvxAsyncCommand StopTimeEntryCommand { get; }

        public IMvxAsyncCommand EditTimeEntryCommand { get; }

        public IMvxAsyncCommand OpenSettingsCommand { get; }

        public IMvxCommand RefreshCommand { get; }

        public MainViewModel(ITogglDataSource dataSource, ITimeService timeService, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.navigationService = navigationService;

            RefreshCommand = new MvxCommand(refresh);
            OpenSettingsCommand = new MvxAsyncCommand(openSettings);
            EditTimeEntryCommand = new MvxAsyncCommand(editTimeEntry);
            StopTimeEntryCommand = new MvxAsyncCommand(stopTimeEntry);
            StartTimeEntryCommand = new MvxAsyncCommand(startTimeEntry);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var tickDisposable = timeService
                .CurrentDateTimeObservable
                .Where(_ => CurrentlyRunningTimeEntry != null)
                .Subscribe(currentTime => CurrentTimeEntryElapsedTime = currentTime - CurrentlyRunningTimeEntry.Start);

            var currentlyRunningTimeEntryDisposable = dataSource.TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(te => CurrentlyRunningTimeEntry = te);

            var syncManagerDisposable = 
                dataSource.SyncManager.StateObservable
                    .Subscribe(syncState => IsSyncing = syncState != SyncState.Sleep);
            
            var spiderDisposable =
                dataSource.TimeEntries.IsEmpty
                    .Subscribe(isEmpty => SpiderIsVisible = !isWelcome && isEmpty);

            disposeBag.Add(tickDisposable);
            disposeBag.Add(spiderDisposable);
            disposeBag.Add(syncManagerDisposable);
            disposeBag.Add(currentlyRunningTimeEntryDisposable);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            navigationService.Navigate<SuggestionsViewModel>();
            navigationService.Navigate<TimeEntriesLogViewModel>();
        }

        private void refresh()
        {
            dataSource.SyncManager.ForceFullSync();
        }

        private Task openSettings()
            => navigationService.Navigate<SettingsViewModel>();

        private Task startTimeEntry() =>
            navigationService.Navigate<StartTimeEntryViewModel, DateParameter>(
                DateParameter.WithDate(timeService.CurrentDateTime)
            );

        private async Task stopTimeEntry()
        {
            await dataSource.TimeEntries.Stop(timeService.CurrentDateTime);
            CurrentTimeEntryElapsedTime = TimeSpan.Zero;
        }

        private Task editTimeEntry()
            => navigationService.Navigate<EditTimeEntryViewModel, IdParameter>(IdParameter.WithId(CurrentlyRunningTimeEntry.Id));
    }
}
