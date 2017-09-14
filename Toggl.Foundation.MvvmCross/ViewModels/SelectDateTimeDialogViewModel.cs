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
        private DateParameter defaultResult;
            
        private DateTime dateTime;
        public DateTime DateTime
        {
            get => dateTime;
            set => dateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand SaveCommand { get; set; }

        public SelectDateTimeDialogViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            CloseCommand = new MvxAsyncCommand(close);
            SaveCommand = new MvxAsyncCommand(save);
        }
        
        public override void Prepare(DateParameter parameter)
        {
            DateTime = parameter.GetDate().UtcDateTime;
            defaultResult = parameter;
        }

        private Task close() => navigationService.Close(this, defaultResult);

        private Task save() => navigationService.Close(this, DateParameter.WithDate(DateTime));
    }
}
