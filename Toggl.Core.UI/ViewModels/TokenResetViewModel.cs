using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Toggl.Networking.Exceptions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TokenResetViewModel : ViewModel
    {
        private readonly ITogglDataSource dataSource;
        private readonly IDialogService dialogService;
        private readonly IUserAccessManager userAccessManager;
        private readonly INavigationService navigationService;
        private readonly IInteractorFactory interactorFactory;

        private bool needsSync;

        public Email Email { get; private set; }
        public IObservable<bool> HasError { get; }
        public IObservable<string> Error { get; }
        public IObservable<bool> NextIsEnabled { get; }
        public ISubject<string> Password { get; } = new BehaviorSubject<string>(string.Empty);

        public UIAction Done { get; private set; }
        public UIAction SignOut { get; private set; }

        public TokenResetViewModel(
            IUserAccessManager userAccessManager,
            ITogglDataSource dataSource,
            IDialogService dialogService,
            INavigationService navigationService,
            IUserPreferences userPreferences,
            IAnalyticsService analyticsService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            IInteractorFactory interactorFactory
        )
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(userAccessManager, nameof(userAccessManager));
            Ensure.Argument.IsNotNull(dialogService, nameof(dialogService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.dataSource = dataSource;
            this.userAccessManager = userAccessManager;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.interactorFactory = interactorFactory;

            Done = rxActionFactory.FromObservable(done);
            SignOut = rxActionFactory.FromAsync(signout);

            Error = Done.Errors
                .Select(transformException);

            HasError = Error
                .Select(error => !string.IsNullOrEmpty(error))
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            NextIsEnabled = Password
                .Select(Shared.Password.From)
                .CombineLatest(Done.Executing, (password, isExecuting) => password.IsValid && !isExecuting)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            needsSync = await dataSource.HasUnsyncedData();
            Email = await dataSource.User.Current.FirstAsync().Select(u => u.Email);
        }

        private async Task signout()
        {
            if (needsSync)
            {
                var userConfirmedLoggingOut = await askToLogOut();
                if (!userConfirmedLoggingOut)
                    return;
            }

            await interactorFactory.Logout(LogoutSource.TokenReset).Execute();
            await navigationService.Navigate<LoginViewModel>();
        }

        private IObservable<Unit> done() =>
            Password
                .FirstAsync()
                .Select(Shared.Password.From)
                .ThrowIf(password => !password.IsValid, new InvalidOperationException())
                .SelectMany(userAccessManager.RefreshToken)
                .Do(onLogin)
                .SelectUnit();

        private void onLogin()
        {
            navigationService.Navigate<MainTabBarViewModel>();
        }

        private string transformException(Exception ex)
        {
            return ex is ForbiddenException
                ? Resources.IncorrectPassword
                : Resources.GenericLoginError;
        }

        private IObservable<bool> askToLogOut()
            => dialogService.Confirm(
                Resources.AreYouSure,
                Resources.SettingsUnsyncedMessage,
                Resources.SettingsDialogButtonSignOut,
                Resources.Cancel);
    }
}
