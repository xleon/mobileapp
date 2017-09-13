using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDateTimeDialogViewModel : MvxViewModel<DateParameter, DateParameter>
    {
        private IMvxNavigationService navigationService;

        private DateTime dateTime;
        public DateTime DateTime
        {
            get => dateTime;
            set => dateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxCommand SaveCommand { get; set; }

        public SelectDateTimeDialogViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            CloseCommand = new MvxAsyncCommand(close);
            SaveCommand = new MvxCommand(save);
        }
        
        public override void Prepare(DateParameter parameter)
        {
            DateTime = parameter.GetDate().LocalDateTime;
        }

        private Task close() => navigationService.Close(this);

        private void save()
            => throw new NotImplementedException();
    }
}
