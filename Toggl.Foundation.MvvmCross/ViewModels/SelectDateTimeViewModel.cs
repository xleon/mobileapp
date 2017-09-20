using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDateTimeViewModel : MvxViewModel<DatePickerParameters, DateTimeOffset>
    {
        private DateTimeOffset defaultResult;

        private readonly IMvxNavigationService navigationService;

        public DateTimeOffset CurrentDateTime { get; set; }

        public DateTimeOffset MinDate { get; private set; }

        public DateTimeOffset MaxDate { get; private set; }

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand SaveCommand { get; set; }

        public SelectDateTimeViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            CloseCommand = new MvxAsyncCommand(close);
            SaveCommand = new MvxAsyncCommand(save);
        }

        private void OnCurrentDateTimeChanged()
        {
            CurrentDateTime = CurrentDateTime.Clamp(MinDate, MaxDate);
        }

        public override void Prepare(DatePickerParameters parameter)
        {
            MinDate = parameter.MinDate;
            MaxDate = parameter.MaxDate;
            CurrentDateTime = defaultResult = parameter.CurrentDate;
        }

        private Task close() => navigationService.Close(this, defaultResult);

        private Task save() => navigationService.Close(this, CurrentDateTime);
    }
}
