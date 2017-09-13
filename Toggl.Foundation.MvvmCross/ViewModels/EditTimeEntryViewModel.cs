using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using PropertyChanged;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using static Toggl.Multivac.Extensions.ObservableExtensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class EditTimeEntryViewModel : MvxViewModel<IdParameter>
    {
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;
        private readonly ITimeService timeService;

        private IDisposable deleteDisposable;
        private IDisposable tickingDisposable;
        private IDisposable confirmDisposable;

        public long Id { get; set; }

        public string Description { get; set; }

        public string Project { get; set; }

        public string ProjectColor { get; set; }

        public string Client { get; set; }

        public string Task { get; set; }

        [DependsOn(nameof(StartTime), nameof(EndTime))]
        public TimeSpan Duration
            => (EndTime ?? timeService.CurrentDateTime) - StartTime;

        public DateTimeOffset StartTime { get; set; }

        private DateTimeOffset? endTime;
        public DateTimeOffset? EndTime
        {
            get => endTime;
            set
            {
                if (endTime == value) return;
                endTime = value;
                if (endTime != null)
                {
                    tickingDisposable?.Dispose();
                    tickingDisposable = null;
                    return;
                }
                subscribeToTimeServiceTicks();
            }
        }

        public List<string> Tags { get; set; }

        public bool Billable { get; set; }

        public IMvxCommand DeleteCommand { get; }

        public IMvxCommand ConfirmCommand { get; }

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand EditDurationCommand { get; }

        public IMvxAsyncCommand SelectStartDateTimeCommand { get; }

        public EditTimeEntryViewModel(ITogglDataSource dataSource, IMvxNavigationService navigationService, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dataSource = dataSource;
            this.navigationService = navigationService;
            this.timeService = timeService;

            DeleteCommand = new MvxCommand(delete);
            ConfirmCommand = new MvxCommand(confirm);
            CloseCommand = new MvxAsyncCommand(close);
            EditDurationCommand = new MvxAsyncCommand(editDuration);
            SelectStartDateTimeCommand = new MvxAsyncCommand(selectStartDateTime);
        }

        public override void Prepare(IdParameter parameter)
        {
            Id = parameter.Id;
        }

        public override async Task Initialize()
        {
            var timeEntry = await dataSource.TimeEntries.GetById(Id);

            Description = timeEntry.Description;
            StartTime = timeEntry.Start;
            EndTime = timeEntry.Stop;
            Billable = timeEntry.Billable;
            Tags = timeEntry.Tags?.Select(tag => tag.Name).ToList() ?? new List<string>();
            Project = timeEntry?.Project?.Name;
            ProjectColor = timeEntry?.Project?.Color;
            Task = timeEntry?.Task?.Name;
            Client = timeEntry?.Project?.Client?.Name;

            if (EndTime == null)
                subscribeToTimeServiceTicks();
        }

        private void subscribeToTimeServiceTicks()
        {
            tickingDisposable = timeService
                .CurrentDateTimeObservable
                .Subscribe(_ => RaisePropertyChanged(nameof(Duration)));
        }

        private void delete()
        {
            deleteDisposable = dataSource.TimeEntries
                .Delete(Id)
                .Subscribe(onDeleteError, onDeleteCompleted);
        }

        private void onDeleteCompleted()
        {
            close();
        }

        private void onDeleteError(Exception exception) { }

        private void confirm()
        {
            var dto = new EditTimeEntryDto { Id = Id, Description = Description };
            confirmDisposable = dataSource.TimeEntries
                                          .Update(dto)
                                          .Subscribe((Exception ex) => close(), () => close());
        }

        private Task close()
            => navigationService.Close(this);

        private Task editDuration()
            => navigationService.Navigate<EditDurationViewModel, DurationParameter>(DurationParameter.WithStartAndStop(StartTime, EndTime));

        private Task selectStartDateTime()
            => navigationService.Navigate<SelectDateTimeDialogViewModel, DateParameter>(DateParameter.WithDate(StartTime));
    }
}
