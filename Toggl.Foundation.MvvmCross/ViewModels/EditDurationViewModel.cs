using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class EditDurationViewModel : MvxViewModel<DurationParameter>
    {
        private readonly IMvxNavigationService navigationService;
        private readonly ITimeService timeService;
        private bool isRunning;

        public DateTimeOffset StartTime { get; private set; }

        public DateTimeOffset StopTime { get; private set; }

        public TimeSpan Duration
        {
            get => StopTime - StartTime;
            set => onDurationChanged(value);
        }

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand SaveCommand { get; }

        public EditDurationViewModel(IMvxNavigationService navigationService, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.navigationService = navigationService;
            this.timeService = timeService;

            CloseCommand = new MvxAsyncCommand(close);
            SaveCommand = new MvxAsyncCommand(save);
        }

        public override async Task Initialize(DurationParameter parameter)
        {
            await base.Initialize();

            isRunning = parameter.Stop == null;
            if (isRunning)
            {
                timeService.CurrentDateTimeObservable
                           .Subscribe(currentTime => StopTime = currentTime);
            }

            StartTime = parameter.Start;
            StopTime = isRunning ? timeService.CurrentDateTime : parameter.Stop.Value;
            Duration = StopTime - StartTime;
        }

        private Task close() => navigationService.Close(this);

        private Task save() => throw new NotImplementedException();

        private void onDurationChanged(TimeSpan duration)
        {
            if (isRunning)
                StartTime = StopTime.Subtract(duration);
            else
                StopTime = StartTime.Add(duration);
        }
    }
}
