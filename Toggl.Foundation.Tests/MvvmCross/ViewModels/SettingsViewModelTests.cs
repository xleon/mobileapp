using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using MvvmCross.Core.Navigation;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class SettingsViewModelTests
    {
        public abstract class SettingsViewModelTest : BaseViewModelTests<SettingsViewModel>
        {
            protected ISubject<SyncState> StateObservableSubject;

            protected override void AdditionalSetup()
            {
                StateObservableSubject = new Subject<SyncState>();
                var syncManager = Substitute.For<ISyncManager>();
                var observable = StateObservableSubject.AsObservable();
                syncManager.StateObservable.Returns(observable);
                DataSource.SyncManager.Returns(syncManager);
            }

            protected override SettingsViewModel CreateViewModel()
                => new SettingsViewModel(DataSource, NavigationService, DialogService);
        }

        public sealed class TheConstructor : SettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useNavigationService, bool useDialogService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var dialogService = useDialogService ? DialogService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SettingsViewModel(dataSource, navigationService, dialogService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheFlags : SettingsViewModelTest
        {
            [Property]
            public void SetsIsRunningSyncCorrectly(NonEmptyArray<SyncState> statuses)
            {
                foreach (var state in statuses.Get)
                {
                    StateObservableSubject.OnNext(state);
                    ViewModel.IsRunningSync.Should().Be(state != SyncState.Sleep);
                }
            }

            [Property]
            public void SetsIsSyncedCorrectly(NonEmptyArray<SyncState> statuses)
            {
                foreach (var state in statuses.Get)
                {
                    StateObservableSubject.OnNext(state);
                    ViewModel.IsSynced.Should().Be(state == SyncState.Sleep);
                }
            }

            [Property]
            public void SetsTheIsRunningSyncAndIsSyncedFlagsToOppositeValues(NonEmptyArray<SyncState> statuses)
            {
                foreach (var state in statuses.Get)
                {
                    StateObservableSubject.OnNext(state);
                    ViewModel.IsRunningSync.Should().Be(!ViewModel.IsSynced);
                }
            }

            [Property]
            public void DoesNotSetTheIsLoggingOutFlagIfTheLogoutCommandIsNotExecuted(NonEmptyArray<SyncState> statuses)
            {
                foreach (var state in statuses.Get)
                {
                    StateObservableSubject.OnNext(state);
                    ViewModel.IsLoggingOut.Should().BeFalse();
                }
            }

            [Property]
            public void DoesNotUnsetTheIsLoggingOutFlagAfterItIsSetNoMatterWhatStatusesAreObserved(NonEmptyArray<SyncState> statuses)
            {
                DataSource.Logout().Returns(Observable.Never<Unit>());

                ViewModel.LogoutCommand.ExecuteAsync();

                foreach (var state in statuses.Get)
                {
                    StateObservableSubject.OnNext(state);
                    ViewModel.IsLoggingOut.Should().BeTrue();
                }
            }

            [Property]
            public void SetsTheIsRunningSyncAndIsSyncedFlagsToFalseAfterTheIsLoggingInFlagIsSetAndDoesNotSetThemToTrueNoMatterWhatStatusesAreObserved(NonEmptyArray<SyncState> statuses)
            {
                DataSource.Logout().Returns(Observable.Never<Unit>());

                ViewModel.LogoutCommand.ExecuteAsync();

                foreach (var state in statuses.Get)
                {
                    StateObservableSubject.OnNext(state);
                    ViewModel.IsRunningSync.Should().BeFalse();
                    ViewModel.IsSynced.Should().BeFalse();
                }
            }
        }

        public sealed class TheLogoutCommand : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task SetsTheIsLoggingOutFlagToTrue()
            {
                doNotShowConfirmationDialog();
                await ViewModel.LogoutCommand.ExecuteAsync();

                ViewModel.IsLoggingOut.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task CallsLogoutOnTheDataSource()
            {
                doNotShowConfirmationDialog();
                await ViewModel.LogoutCommand.ExecuteAsync();

                await DataSource.Received().Logout();
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheOnboardingScreen()
            {
                doNotShowConfirmationDialog();
                await ViewModel.LogoutCommand.ExecuteAsync();

                await NavigationService.Received().Navigate(typeof(OnboardingViewModel));
            }

            [Fact, LogIfTooSlow]
            public void ChecksIfThereAreUnsyncedDataWhenTheSyncProcessFinishes()
            {
                StateObservableSubject.OnNext(SyncState.Sleep);

                DataSource.Received().HasUnsyncedData();
            }

            [Fact, LogIfTooSlow]
            public void SetsTheIsSyncedFlagAfterTheSyncProcessHasFinishedAndThereIsNoTimeEntryToPush()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                StateObservableSubject.OnNext(SyncState.Sleep);

                ViewModel.IsSynced.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void UnsetsTheIsSyncedFlagWhenTheSyncProcessIsNotRunningButThrereIsSomeTimeEntryToPush()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(true));
                StateObservableSubject.OnNext(SyncState.Sleep);

                ViewModel.IsSynced.Should().BeFalse();
            }

            [Theory, LogIfTooSlow]
            [InlineData(SyncState.Pull)]
            [InlineData(SyncState.Push)]
            public void UnsetsTheIsSyncedFlagWhenThereIsNothingToPushButTheSyncProcessStartsAgain(SyncState state)
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                StateObservableSubject.OnNext(SyncState.Sleep);
                StateObservableSubject.OnNext(state);

                ViewModel.IsSynced.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotShowConfirmationDialogWhenTheAppIsInSync()
            {
                doNotShowConfirmationDialog();

                await ViewModel.LogoutCommand.ExecuteAsync();

                await DialogService.DidNotReceiveWithAnyArgs().Confirm("", "", "", "");
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsConfirmationDialogWhenThereIsNothingToPushButSyncIsRunning()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                StateObservableSubject.OnNext(SyncState.Pull);

                await ViewModel.LogoutCommand.ExecuteAsync();

                await DialogService.ReceivedWithAnyArgs().Confirm("", "", "", "");
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsConfirmationDialogWhenThereIsSomethingToPushButSyncIsNotRunning()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(true));
                StateObservableSubject.OnNext(SyncState.Sleep);

                await ViewModel.LogoutCommand.ExecuteAsync();

                await DialogService.ReceivedWithAnyArgs().Confirm("", "", "", "");
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotProceedWithLogoutWhenUserClicksCancelButtonInTheDialog()
            {
                StateObservableSubject.OnNext(SyncState.Pull);
                DialogService.Confirm(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>()).Returns(false);

                await ViewModel.LogoutCommand.ExecuteAsync();

                ViewModel.IsLoggingOut.Should().BeFalse();
                await DataSource.DidNotReceive().Logout();
                await NavigationService.DidNotReceive().Navigate<OnboardingViewModel>();
            }

            [Fact, LogIfTooSlow]
            public async Task ProceedsWithLogoutWhenUserClicksSignOutButtonInTheDialog()
            {
                StateObservableSubject.OnNext(SyncState.Pull);
                DialogService.Confirm(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>()).Returns(true);

                await ViewModel.LogoutCommand.ExecuteAsync();

                ViewModel.IsLoggingOut.Should().BeTrue();
                await DataSource.Received().Logout();
                await NavigationService.Received().Navigate<OnboardingViewModel>();
            }

            private void doNotShowConfirmationDialog()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                StateObservableSubject.OnNext(SyncState.Sleep);
            }
        }

        public sealed class ThePickWorkspaceCommand : SettingsViewModelTest
        {
            private const long workspaceId = 10;
            private const long defaultWorkspaceId = 11;
            private const string workspaceName = "My custom workspace";
            private readonly IDatabaseWorkspace workspace = Substitute.For<IDatabaseWorkspace>();
            private readonly IDatabaseWorkspace defaultWorkspace = Substitute.For<IDatabaseWorkspace>();

            public ThePickWorkspaceCommand()
            {
                workspace.Id.Returns(workspaceId);
                workspace.Name.Returns(workspaceName);
                defaultWorkspace.Id.Returns(defaultWorkspaceId);

                DataSource.Workspaces.GetDefault()
                    .Returns(Observable.Return(defaultWorkspace));

                DataSource.Workspaces.GetById(workspaceId)
                    .Returns(Observable.Return(workspace));

                ViewModel.Prepare();
            }

            [Fact, LogIfTooSlow]
            public async Task CallsTheSelectWorkspaceViewModel()
            {
                await ViewModel.EditWorkspaceCommand.ExecuteAsync();

                await NavigationService.Received()
                    .Navigate<WorkspaceParameters, long>(typeof(SelectWorkspaceViewModel), Arg.Any<WorkspaceParameters>());
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheReturnedWorkspaceNameAsTheWorkspaceNameProperty()
            {
                NavigationService
                    .Navigate<WorkspaceParameters, long>(typeof(SelectWorkspaceViewModel), Arg.Any<WorkspaceParameters>())
                    .Returns(Task.FromResult(workspaceId));

                await ViewModel.EditWorkspaceCommand.ExecuteAsync();

                ViewModel.WorkspaceName.Should().Be(workspaceName);
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheUserWithTheReceivedWorspace()
            {
                NavigationService
                    .Navigate<WorkspaceParameters, long>(typeof(SelectWorkspaceViewModel), Arg.Any<WorkspaceParameters>())
                    .Returns(Task.FromResult(workspaceId));

                await ViewModel.EditWorkspaceCommand.ExecuteAsync();

                await DataSource.User.Received().UpdateWorkspace(Arg.Is(workspaceId));
            }

            [Fact, LogIfTooSlow]
            public async Task StartsTheSyncAlgorithm()
            {
                NavigationService
                    .Navigate<WorkspaceParameters, long>(typeof(SelectWorkspaceViewModel), Arg.Any<WorkspaceParameters>())
                    .Returns(Task.FromResult(workspaceId));

                await ViewModel.EditWorkspaceCommand.ExecuteAsync();

                await DataSource.SyncManager.Received().PushSync();
            }
        }

        public sealed class TheBackCommand : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.BackCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }

        public sealed class TheToggleAddMobileTagCommand : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void TogglesTheCurrentValueOfTheToggleAddMobileTagProperty()
            {
                var expected = !ViewModel.AddMobileTag;

                ViewModel.ToggleAddMobileTagCommand.Execute();

                ViewModel.AddMobileTag.Should().Be(expected);
            }
        }

        public sealed class TheToggleUseTwentyFourHourClockCommand : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void TogglesTheCurrentValueOfTheToggleUseTwentyFourHourClockProperty()
            {
                var expected = !ViewModel.UseTwentyFourHourClock;

                ViewModel.ToggleUseTwentyFourHourClockCommand.Execute();

                ViewModel.UseTwentyFourHourClock.Should().Be(expected);
            }
        }
    }
}
