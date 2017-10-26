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
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class SettingsViewModelTests
    {
        public abstract class SettingsViewModelTest : BaseViewModelTests<SettingsViewModel>
        {
            protected override SettingsViewModel CreateViewModel()
                => new SettingsViewModel(DataSource, NavigationService);
        }

        public sealed class TheConstructor : SettingsViewModelTest
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SettingsViewModel(dataSource, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheIsRunningSyncObservable : SettingsViewModelTest
        {
            private ISubject<SyncState> subject;

            protected override void AdditionalSetup()
            {
                subject = new Subject<SyncState>();
                var syncManager = Substitute.For<ISyncManager>();
                var observable = subject.AsObservable();
                syncManager.StateObservable.Returns(observable);
                DataSource.SyncManager.Returns(syncManager);
            }

            [Property]
            public void EmitsTrueAndFalseWhenASyncManagerStateIsChanged(NonEmptyArray<SyncState> statuses)
            {
                var log = new List<bool>();
                var expectedBooleans = new List<bool>();

                foreach (var state in statuses.Get)
                {
                    subject.OnNext(state);
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
                await ViewModel.LogoutCommand.ExecuteAsync();

                ViewModel.IsLoggingOut.Should().BeTrue();
            }

            [Fact]
            public async Task CallsFreezeOnTheSyncManager()
            {
                await ViewModel.LogoutCommand.ExecuteAsync();

                await DataSource.SyncManager.Received().Freeze();
            }

            [Fact]
            public async Task CallsLogoutOnTheDataSource()
            {
                await ViewModel.LogoutCommand.ExecuteAsync();

                await DataSource.Received().Logout();
            }

            [Fact]
            public async Task NavigatesToTheOnboardingScreen()
            {
                await ViewModel.LogoutCommand.ExecuteAsync();

                await NavigationService.Received().Navigate(typeof(OnboardingViewModel));
            }

            [Fact]
            public async Task DoesOperationsInTheCorrectOrder()
            {
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
