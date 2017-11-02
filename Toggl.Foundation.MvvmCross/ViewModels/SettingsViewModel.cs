using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Sync;
using Toggl.Multivac;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SettingsViewModel : MvxViewModel
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;
        private readonly IDialogService dialogService;

        public string Title { get; private set; }

        public bool IsLoggingOut { get; private set; }

        public bool IsRunningSync { get; private set; }

        public bool IsSynced { get; private set; }

        public IMvxAsyncCommand LogoutCommand { get; }

        public IMvxAsyncCommand BackCommand { get; }

        public SettingsViewModel(ITogglDataSource dataSource, IMvxNavigationService navigationService, IDialogService dialogService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(dialogService, nameof(dialogService));

            Title = Resources.Settings;

            this.dataSource = dataSource;
            this.navigationService = navigationService;
            this.dialogService = dialogService;

            IsLoggingOut = false;
            IsSynced = false;
            IsRunningSync = false;

            var syncDisposable = dataSource.SyncManager
                .StateObservable
                .Subscribe(async state =>
                {
                    IsRunningSync = IsLoggingOut == false && state != SyncState.Sleep;
                    IsSynced = IsLoggingOut == false && await isSynced();
                });

            BackCommand = new MvxAsyncCommand(back);
            LogoutCommand = new MvxAsyncCommand(maybeLogout);

            disposeBag.Add(syncDisposable);
        }

        private Task back() => navigationService.Close(this);

        private async Task maybeLogout()
        {
            if (await isSynced())
            {
                await logout();
                return;
            }

            var (title, message) = IsRunningSync
                ? (Resources.SettingsSyncInProgressTitle, Resources.SettingsSyncInProgressMessage)
                : (Resources.SettingsUnsyncedTitle, Resources.SettingsUnsyncedMessage);

            dialogService.Confirm(
                title,
                message,
                Resources.SettingsDialogButtonSignOut,
                Resources.Cancel,
                async () => await logout(),
                dismissAction: null,
                makeConfirmActionBold: true
            );
        }

        private async Task logout()
        {
            IsLoggingOut = true;
            IsSynced = false;
            IsRunningSync = false;
            await dataSource.SyncManager.Freeze();
            await dataSource.Logout();
            await navigationService.Navigate<OnboardingViewModel>();
        }

        private async Task<bool> isSynced()
            => IsRunningSync ? false : await thereIsNothingToSync();

        private async Task<bool> thereIsNothingToSync()
            => await dataSource.TimeEntries.GetAll(te => te.SyncStatus != SyncStatus.InSync).SelectMany(te => te).IsEmpty();
    }
}
