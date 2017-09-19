using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDateTimeViewModel : MvxViewModel<DateTimeOffset, DateTimeOffset>
    {
        private readonly IMvxNavigationService navigationService;
        private DateTimeOffset defaultResult;

        public DateTimeOffset DateTimeOffset { get; set; }

        public IMvxAsyncCommand CloseCommand { get; }

        public IMvxAsyncCommand SaveCommand { get; set; }

        public SelectDateTimeViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            CloseCommand = new MvxAsyncCommand(close);
            SaveCommand = new MvxAsyncCommand(save); 
        }
        
        public override void Prepare(DateTimeOffset parameter)
        {
            DateTimeOffset = defaultResult = parameter;
        }

        private Task close() => navigationService.Close(this, defaultResult);

        private Task save() => navigationService.Close(this, DateTimeOffset);
    }
}
