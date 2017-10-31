using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using MvvmCross.Core.Navigation;
using NSubstitute;
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
            [Theory]
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

        public sealed class TheIsRunningSyncObservable : SettingsViewModelTest
        {
            [Property]
            public void EmitsTrueAndFalseWhenASyncManagerStateIsChanged(NonEmptyArray<SyncState> statuses)
            {
                var log = new List<bool>();
                var expectedBooleans = new List<bool>();

                foreach (var state in statuses.Get)
                {
                    StateObservableSubject.OnNext(state);
                    expectedBooleans.Add(state != SyncState.Sleep);
                    log.Add(ViewModel.IsRunningSync);
                }

                log.Should().BeEquivalentTo(expectedBooleans);
            }
        }

        public sealed class TheLogoutCommand : SettingsViewModelTest
        {
            [Fact]
            public async Task SetsTheIsLoggingOutFlagToTrue()
            {
                doNotShowConfirmationDialog();
                await ViewModel.LogoutCommand.ExecuteAsync();

                ViewModel.IsLoggingOut.Should().BeTrue();
            }

            [Fact]
            public async Task CallsFreezeOnTheSyncManager()
            {
                doNotShowConfirmationDialog();
                await ViewModel.LogoutCommand.ExecuteAsync();

                await DataSource.SyncManager.Received().Freeze();
            }

            [Fact]
            public async Task CallsLogoutOnTheDataSource()
            {
                doNotShowConfirmationDialog();
                await ViewModel.LogoutCommand.ExecuteAsync();

                await DataSource.Received().Logout();
            }

            [Fact]
            public async Task NavigatesToTheOnboardingScreen()
            {
                doNotShowConfirmationDialog();
                await ViewModel.LogoutCommand.ExecuteAsync();

                await NavigationService.Received().Navigate(typeof(OnboardingViewModel));
            }

            [Fact]
            public async Task DoesOperationsInTheCorrectOrder()
            {
                doNotShowConfirmationDialog();
                int operationCounter = 0;
                int isLoggingOutFlag = -1;
                int callingSyncManagerFreeze = -1;
                int awaitingSyncManagerFreeze = -1;
                int callingLogout = -1;
                int awaitingLogout = -1;
                int callingNavigate = -1;
                int awaitingNavigation = -1;
                var syncManager = Substitute.For<ISyncManager>();
                syncManager.Freeze().Returns(Observable.Create<SyncState>(async observer =>
                {
                    if (ViewModel.IsLoggingOut == true)
                        isLoggingOutFlag = operationCounter++;

                    callingSyncManagerFreeze = operationCounter++;
                    await Task.Delay(100);
                    observer.OnNext(SyncState.Sleep);
                    observer.OnCompleted();
                    awaitingSyncManagerFreeze = operationCounter++;
                    return () => { };
                }));
                DataSource.SyncManager.Returns(syncManager);
                DataSource.Logout().Returns(Observable.Create<Unit>(async observer =>
                {
                    callingLogout = operationCounter++;
                    await Task.Delay(100);
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                    awaitingLogout = operationCounter++;
                    return () => { };
                }));
                NavigationService.Navigate<OnboardingViewModel>().Returns(_ => Task.Run(async () =>
                {
                    callingNavigate = operationCounter++;
                    await Task.Delay(100);
                    awaitingNavigation = operationCounter++;
                }));

                await ViewModel.LogoutCommand.ExecuteAsync();
        
                isLoggingOutFlag.Should().Be(0);
                callingSyncManagerFreeze.Should().Be(1);
                awaitingSyncManagerFreeze.Should().Be(2);
                callingLogout.Should().Be(3);
                awaitingLogout.Should().Be(4);
                callingNavigate.Should().Be(5);
                awaitingNavigation.Should().Be(6);
            }

            [Fact]
            public async Task DoesNotShowConfirmationDialogWhenTheAppIsInSync()
            {
                doNotShowConfirmationDialog();

                await ViewModel.LogoutCommand.ExecuteAsync();

                DialogService.DidNotReceiveWithAnyArgs().Confirm("", "", "", "", null, null, false);
            }

            [Fact]
            public async Task ShowsConfirmationDialogWhenThereIsNothingToPushButSyncIsRunning()
            {
                var emptyList = Observable.Return(new IDatabaseTimeEntry[0]);
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(emptyList);
                StateObservableSubject.OnNext(SyncState.Pull);

                await ViewModel.LogoutCommand.ExecuteAsync();

                DialogService.ReceivedWithAnyArgs().Confirm("", "", "", "", null, null, false);
            }

            [Fact]
            public async Task ShowsConfirmationDialogWhenThereIsSomethingToPushButSyncIsNotRunning()
            {
                var listOfTimeEntries = Observable.Return(new[] { Substitute.For<IDatabaseTimeEntry>() });
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(listOfTimeEntries);
                StateObservableSubject.OnNext(SyncState.Sleep);

                await ViewModel.LogoutCommand.ExecuteAsync();

                DialogService.ReceivedWithAnyArgs().Confirm("", "", "", "", null, null, false);
            }

            [Fact]
            public async Task DoesNotProceedWithLogoutWhenUserClicksCancelButtonInTheDialog()
            {
                StateObservableSubject.OnNext(SyncState.Pull);
                DialogService.Confirm(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<Action>(), Arg.Do<Action>(action => action?.Invoke()), Arg.Any<bool>());

                await ViewModel.LogoutCommand.ExecuteAsync();

                ViewModel.IsLoggingOut.Should().BeFalse();
                await DataSource.SyncManager.DidNotReceive().Freeze();
                await DataSource.DidNotReceive().Logout();
                await NavigationService.DidNotReceive().Navigate<OnboardingViewModel>();
            }

            [Fact]
            public async Task ProceedsWithLogoutWhenUserClicksSignOutButtonInTheDialog()
            {
                StateObservableSubject.OnNext(SyncState.Pull);
                DialogService.Confirm(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Invoke(), Arg.Any<Action>(), Arg.Any<bool>());

                await ViewModel.LogoutCommand.ExecuteAsync();

                ViewModel.IsLoggingOut.Should().BeTrue();
                await DataSource.SyncManager.Received().Freeze();
                await DataSource.Received().Logout();
                await NavigationService.Received().Navigate<OnboardingViewModel>();
            }

            private void doNotShowConfirmationDialog()
            {
                var emptyList = Observable.Return(new IDatabaseTimeEntry[0]);
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(_ => emptyList);
                StateObservableSubject.OnNext(SyncState.Sleep);
            }
        }

        public sealed class TheBackCommand : SettingsViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.BackCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }
    }
}
