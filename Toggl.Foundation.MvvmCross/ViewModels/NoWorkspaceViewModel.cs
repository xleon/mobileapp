using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.Sync;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class NoWorkspaceViewModel : MvxViewModel
    {
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly IMvxNavigationService navigationService;
        private readonly Subject<bool> isLoading = new Subject<bool>();

        public IObservable<bool> IsLoading => isLoading.AsObservable();

        public NoWorkspaceViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.navigationService = navigationService;
            this.interactorFactory = interactorFactory;
        }

        public async Task TryAgain()
        {
            isLoading.OnNext(true);

            dataSource.CreateNewSyncManager();

            var workspaces = await dataSource
                .SyncManager
                .ForceFullSync()
                .Where(state => state == SyncState.Sleep)
                .SelectMany(dataSource.Workspaces.GetAll());

            isLoading.OnNext(false);

            if (workspaces.Any())
            {
                close();
            }
        }

        public async Task CreateWorkspaceWithDefaultName()
        {
            isLoading.OnNext(true);

            dataSource.CreateNewSyncManager();

            await interactorFactory.CreateDefaultWorkspace().Execute();

            await dataSource
                .SyncManager
                .ForceFullSync();

            isLoading.OnNext(false);
            close();
        }

        private void close()
        {
            navigationService.Close(this);
        }
    }
}
