using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class StartTimeEntryViewModel : MvxViewModel<DateParameter>
    {
        //Fields
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;

        private IDisposable elapsedTimeDisposable;

        //Properties
        public string RawTimeEntryText { get; set; } = "";

        public int CursorPosition { get; set; } = 0;

        public TimeSpan ElapsedTime { get; private set; } = TimeSpan.Zero;

        public bool IsBillable { get; private set; } = false;

        public bool IsEditingProjects { get; private set; } = false;

        public bool IsEditingTags { get; private set; } = false;

        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset? EndDate { get; private set; }

        public ObservableCollection<ITimeEntrySuggestionViewModel> Suggestions { get; }
            = new ObservableCollection<ITimeEntrySuggestionViewModel>();

        public IMvxAsyncCommand BackCommand { get; }

        public IMvxAsyncCommand DoneCommand { get; }

        public IMvxCommand ToggleBillableCommand { get; }

        public StartTimeEntryViewModel(ITogglDataSource dataSource, ITimeService timeService, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.navigationService = navigationService;

            BackCommand = new MvxAsyncCommand(back);
            DoneCommand = new MvxAsyncCommand(done);
            ToggleBillableCommand = new MvxCommand(toggleBillable);
        }

        public override async Task Initialize(DateParameter parameter)
        {
            await Initialize();

            StartDate = parameter.GetDate();

            elapsedTimeDisposable =
                timeService.CurrentDateTimeObservable.Subscribe(currentTime => ElapsedTime = currentTime - StartDate);
        }

        private void toggleBillable() => IsBillable = !IsBillable;

        private Task back() => navigationService.Close(this);

        private async Task done()
        {
            await dataSource.TimeEntries.Start(StartDate, RawTimeEntryText, IsBillable);

            await navigationService.Close(this);
        }
    }
}
