using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class EditDurationViewModel : MvxViewModel<DurationParameter, DurationParameter>
    {
        private readonly ITimeService timeService;
        private readonly IMvxNavigationService navigationService;

        private bool isRunning;
        private DurationParameter defaultResult;

        public DateTimeOffset StartTime { get; private set; }

        public DateTimeOffset StopTime { get; private set; }

        public TimeSpan Duration
        {
            get => StopTime - StartTime;
            set => onDurationChanged(value);
        }

        public IMvxAsyncCommand SaveCommand { get; }

        public IMvxAsyncCommand CloseCommand { get; }

        public EditDurationViewModel(IMvxNavigationService navigationService, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;
            this.navigationService = navigationService;

            SaveCommand = new MvxAsyncCommand(save);
            CloseCommand = new MvxAsyncCommand(close);
        }

        public override void Prepare(DurationParameter parameter)
        {
            defaultResult = parameter;

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

        private Task close()
            => navigationService.Close(this, defaultResult);

        private Task save()
            => navigationService.Close(this, DurationParameter.WithStartAndStop(StartTime, StopTime));

        private void onDurationChanged(TimeSpan duration)
        {
            if (isRunning)
                StartTime = StopTime.Subtract(duration);
            else
                StopTime = StartTime.Add(duration);
        }
    }
}
