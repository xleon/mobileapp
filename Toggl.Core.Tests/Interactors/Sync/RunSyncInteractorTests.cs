using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.Diagnostics;
using Toggl.Core.Interactors;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Generators;
using Xunit;
using Notification = Toggl.Shared.Notification;
using SyncOutcome = Toggl.Core.Models.SyncOutcome;
using SyncState = Toggl.Core.Sync.SyncState;

namespace Toggl.Core.Tests.Interactors.Workspace
{
    public sealed class RunSyncInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useSyncManager, bool useSyncManagerAction, bool userStopwatchProvider)
            {
                Action tryingToConstructWithNull = () =>
                    new RunSyncInteractor(
                        useSyncManager ? SyncManager : null,
                        userStopwatchProvider ? StopwatchProvider : null,
                        useSyncManagerAction ? Substitute.For<Func<ISyncManager, IObservable<SyncState>>>() : null,
                        MeasuredOperation.Sync,
                        null,
                        null,
                        null
                    );

                tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
            }
        }

        public class TheExecuteMethod : BaseInteractorTests
        {
            protected readonly IAnalyticsEvent SyncStartedAnalyticsEvent = Substitute.For<IAnalyticsEvent>();
            protected readonly IAnalyticsEvent<string> SyncFinishedAnalyticsEvent = Substitute.For<IAnalyticsEvent<string>>();
            protected readonly IAnalyticsEvent<string, string, string> SyncFailedAnalyticsEvent = Substitute.For<IAnalyticsEvent<string, string, string>>();

            public static IEnumerable<object[]> SyncMethods =>
                new List<object[]>
                {
                    new object[] { new NamedFuncHolder<ISyncManager, IObservable<SyncState>>("ForceFullSync", manager => manager.ForceFullSync()) },
                    new object[] { new NamedFuncHolder<ISyncManager, IObservable<SyncState>>("PullTimeEntries", manager => manager.PullTimeEntries()) },
                    new object[] { new NamedFuncHolder<ISyncManager, IObservable<SyncState>>("PushSync", manager => manager.PushSync()) },
                    new object[] { new NamedFuncHolder<ISyncManager, IObservable<SyncState>>("CleanUp", manager => manager.CleanUp()) },
                };

            public RunSyncInteractor CreateSyncInteractor(
                Func<ISyncManager, IObservable<SyncState>> syncAction,
                MeasuredOperation measuredSyncOperation = MeasuredOperation.None,
                IAnalyticsEvent syncStartedAnalyticsEvent = null,
                IAnalyticsEvent<string> syncFinishedAnalyticsEvent = null,
                IAnalyticsEvent<string, string, string> syncFailedAnalyticsEvent = null)
            {
                return new RunSyncInteractor(
                    SyncManager,
                    StopwatchProvider,
                    syncAction,
                    measuredSyncOperation,
                    syncStartedAnalyticsEvent,
                    syncFinishedAnalyticsEvent,
                    syncFailedAnalyticsEvent);
            }
        }

        public class WhenSyncFails : TheExecuteMethod
        {
            public WhenSyncFails()
            {
                SyncManager.ForceFullSync().Returns(Observable.Throw<SyncState>(new Exception()));
                SyncManager.PullTimeEntries().Returns(Observable.Throw<SyncState>(new Exception()));
                SyncManager.PushSync().Returns(Observable.Throw<SyncState>(new Exception()));
                SyncManager.CleanUp().Returns(Observable.Throw<SyncState>(new Exception()));
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SyncMethods))]
            public async Task ReturnsFailedIfSyncFails(NamedFuncHolder<ISyncManager, IObservable<SyncState>> syncAction)
            {
                var interactor = CreateSyncInteractor(syncAction.Func);
                (await interactor.Execute().SingleAsync()).Should().Be(SyncOutcome.Failed);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksIfSyncFails()
            {
                var exception = new Exception();
                SyncManager.ForceFullSync().Returns(Observable.Throw<SyncState>(exception));
                var interactor = CreateSyncInteractor(manager => manager.ForceFullSync(), MeasuredOperation.None, SyncStartedAnalyticsEvent, SyncFinishedAnalyticsEvent, SyncFailedAnalyticsEvent);

                await interactor.Execute().SingleAsync();

                SyncStartedAnalyticsEvent.Received().Track();
                SyncFinishedAnalyticsEvent.Received().Track(nameof(SyncOutcome.Failed));
                SyncFailedAnalyticsEvent.Received()
                    .Track(exception.GetType().FullName, exception.Message, exception.StackTrace);
            }
        }

        public class WhenSyncSucceeds : TheExecuteMethod
        {
            public WhenSyncSucceeds()
            {
                SyncManager.ForceFullSync().Returns(Observable.Return(SyncState.Sleep));
                SyncManager.PullTimeEntries().Returns(Observable.Return(SyncState.Sleep));
                SyncManager.PushSync().Returns(Observable.Return(SyncState.Sleep));
                SyncManager.CleanUp().Returns(Observable.Return(SyncState.Sleep));
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SyncMethods))]
            public async Task ReturnsNewDataIfSyncSucceeds(NamedFuncHolder<ISyncManager, IObservable<SyncState>> syncAction)
            {
                var interactor = CreateSyncInteractor(syncAction.Func);
                (await interactor.Execute().SingleAsync()).Should().Be(SyncOutcome.NewData);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksIfSyncSucceeds()
            {
                SyncManager.ForceFullSync().Returns(Observable.Return(SyncState.Sleep));
                var interactor = CreateSyncInteractor(manager => manager.ForceFullSync(), MeasuredOperation.None, SyncStartedAnalyticsEvent, SyncFinishedAnalyticsEvent, SyncFailedAnalyticsEvent);

                await interactor.Execute().SingleAsync();

                SyncStartedAnalyticsEvent.Received().Track();
                SyncFinishedAnalyticsEvent.Received().Track(nameof(SyncOutcome.NewData));
                SyncFailedAnalyticsEvent.DidNotReceive().Track(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            }
        }

        [Serializable]
        public struct NamedFuncHolder<T, T2>
        {
            private readonly string name;
            public Func<T, T2> Func { get; }

            public NamedFuncHolder(string name, Func<T, T2> func)
            {
                this.name = name;
                Func = func;
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
