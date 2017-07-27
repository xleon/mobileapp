using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class StartTimeEntryViewModel : BaseViewModel<DateParameter>
    {
        //Fields
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;

        //Properties
        public string RawTimeEntryText { get; set; } = "";

        public int CursorPosition { get; set; } = 0;

        public bool IsBillable { get; private set; } = false;

        public bool IsEditingProjects { get; private set; } = false;

        public bool IsEditingTags { get; private set; } = false;

        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset? EndDate { get; private set; }

        public ObservableCollection<IBaseModel> Suggestions { get; } = new ObservableCollection<IBaseModel>();

        public IMvxAsyncCommand BackCommand { get; }

        public StartTimeEntryViewModel(ITogglDataSource dataSource, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.navigationService = navigationService;

            BackCommand = new MvxAsyncCommand(back);
        }
       
        public override async Task Initialize(DateParameter parameter)
        {
            await Initialize();

            StartDate = parameter.GetDate();
        }

        private Task back()
            => navigationService.Close(this);
    }
}
