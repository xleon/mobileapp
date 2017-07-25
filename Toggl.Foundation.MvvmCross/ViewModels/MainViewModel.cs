using System;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        private readonly IMvxNavigationService navigationService;

        public IMvxAsyncCommand StartTimeEntryCommand { get; }

        public MainViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            StartTimeEntryCommand = new MvxAsyncCommand(startTimeEntry);
        }

        public override void Appeared()
        {
            base.Appeared();
            navigationService.Navigate<SuggestionsViewModel>();
            navigationService.Navigate<TimeEntriesLogViewModel>();
        }

        private async Task startTimeEntry() {
            await navigationService.Navigate<StartTimeEntryViewModel, DateParameter>(
                DateParameter.WithDate(DateTime.UtcNow)
            );
        }
    }
}
