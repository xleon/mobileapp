using Microsoft.Reactive.Testing;
using MvvmCross.Navigation;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.Tests.UI
{
    public abstract class BaseMvvmCrossTests : ReactiveTest
    {
        protected IIdProvider IdProvider { get; } = Substitute.For<IIdProvider>();
        protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
        protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();
        protected IUserPreferences UserPreferences { get; } = Substitute.For<IUserPreferences>();
        protected ICalendarService CalendarService { get; } = Substitute.For<ICalendarService>();
        protected IAnalyticsService AnalyticsService { get; } = Substitute.For<IAnalyticsService>();
        protected IInteractorFactory InteractorFactory { get; } = Substitute.For<IInteractorFactory>();
        protected IPermissionsService PermissionsService { get; } = Substitute.For<IPermissionsService>();
        protected IApplicationShortcutCreator ApplicationShortcutCreator { get; }
            = Substitute.For<IApplicationShortcutCreator>();

        protected IMvxNavigationService NavigationService { get; } = Substitute.For<IMvxNavigationService>();
        protected TestSchedulerProvider SchedulerProvider { get; } = new TestSchedulerProvider();
        protected IIntentDonationService IntentDonationService { get; } = Substitute.For<IIntentDonationService>();
        protected IPrivateSharedStorageService PrivateSharedStorageService { get; } = Substitute.For<IPrivateSharedStorageService>();
    }
}
