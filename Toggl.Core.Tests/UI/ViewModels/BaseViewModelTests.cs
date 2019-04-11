using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using MvvmCross.ViewModels;
using NSubstitute;
using Toggl.Core.Autocomplete;
using Toggl.Core.Diagnostics;
using Toggl.Core.Login;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Core.Suggestions;
using Toggl.Core.Sync;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Settings;
using Toggl.Networking;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public abstract class BaseViewModelTests<TViewModel> : BaseMvvmCrossTests
        where TViewModel : MvxViewModel
    {
        protected ITogglApi Api { get; } = Substitute.For<ITogglApi>();
        protected IApiFactory ApiFactory { get; } = Substitute.For<IApiFactory>();
        protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
        protected ISyncManager SyncManager { get; } = Substitute.For<ISyncManager>();
        protected IUserAccessManager UserAccessManager { get; } = Substitute.For<IUserAccessManager>();
        protected IRatingService RatingService { get; } = Substitute.For<IRatingService>();
        protected IDialogService DialogService { get; } = Substitute.For<IDialogService>();
        protected IBrowserService BrowserService { get; } = Substitute.For<IBrowserService>();
        protected ILicenseProvider LicenseProvider { get; } = Substitute.For<ILicenseProvider>();
        protected IBackgroundService BackgroundService { get; } = Substitute.For<IBackgroundService>();
        protected IPlatformInfo PlatformInfo { get; } = Substitute.For<IPlatformInfo>();
        protected IOnboardingStorage OnboardingStorage { get; } = Substitute.For<IOnboardingStorage>();
        protected IRemoteConfigService RemoteConfigService { get; } = Substitute.For<IRemoteConfigService>();
        protected IPasswordManagerService PasswordManagerService { get; } = Substitute.For<IPasswordManagerService>();
        protected IErrorHandlingService ErrorHandlingService { get; } = Substitute.For<IErrorHandlingService>();

        protected ISuggestionProviderContainer SuggestionProviderContainer { get; } =
            Substitute.For<ISuggestionProviderContainer>();
        protected IAccessRestrictionStorage AccessRestrictionStorage { get; } = Substitute.For<IAccessRestrictionStorage>();
        protected IStopwatchProvider StopwatchProvider { get; } = Substitute.For<IStopwatchProvider>();

        protected TestScheduler TestScheduler { get; }
        protected IRxActionFactory RxActionFactory { get; }

        protected TViewModel ViewModel { get; private set; }

        protected BaseViewModelTests()
        {
            TestScheduler = SchedulerProvider.TestScheduler;
            RxActionFactory = new RxActionFactory(SchedulerProvider);

            Setup();
        }

        protected abstract TViewModel CreateViewModel();

        private void Setup()
        {
            AdditionalSetup();

            ViewModel = CreateViewModel();

            AdditionalViewModelSetup();
        }

        protected virtual void AdditionalSetup()
        {
        }

        protected virtual void AdditionalViewModelSetup()
        {
        }
    }
}
