using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.Tests.Mocks;
using Toggl.Storage;
using Toggl.Storage.Models;
using Toggl.Shared.Models;
using Xunit;
using ITimeEntryPrototype = Toggl.Core.Models.ITimeEntryPrototype;
using Toggl.Core.Interactors;
using System.Reactive;
using System.Collections.Generic;
using Toggl.Shared.Extensions;
using System.Linq;
using Toggl.Core.DataSources;

namespace Toggl.Core.Tests.Interactors
{
    public sealed class SoftDeleteMultipleTimeEntriesInteractorTests
    {
        public class SoftDeleteMultipleTimeEntriesInteractorTest : BaseInteractorTests
        {
            private long[] ids;
            private IInteractor<IObservable<Unit>> testedInteractor;
            private IInteractorFactory interactorFactory;
            private ITimeEntriesSource dataSource;

            private void setupTest()
            {
                ids = new long[] { 4, 8, 15, 16, 23, 42 };

                interactorFactory = Substitute.For<IInteractorFactory>();

                var teInteractor = Substitute.For<IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>>>();
                var workspace = Substitute.For<IThreadSafeWorkspace>();
                var tes = Enumerable.Range(0, 5)
                    .Select((_, index) => new MockTimeEntry(index, workspace))
                    .ToList();
                var observable = Observable.Return(tes);
                teInteractor.Execute().Returns(observable);
                dataSource = Substitute.For<ITimeEntriesSource>();
                DataSource.TimeEntries.Returns(dataSource);

                interactorFactory
                    .GetMultipleTimeEntriesById(Arg.Any<long[]>())
                    .Returns(teInteractor);

                testedInteractor = new SoftDeleteMultipleTimeEntriesInteractor(DataSource.TimeEntries, SyncManager, interactorFactory, ids);
            }

            [Fact, LogIfTooSlow]
            public void PropagatesCorrectIdsToGetMultipleTimeEntriesInteractor()
            {
                setupTest();

                testedInteractor.Execute().Wait();

                interactorFactory.Received().GetMultipleTimeEntriesById(
                    Arg.Is<long[]>(propagatedIds => propagatedIds.SetEquals(ids, null)));
            }

            [Fact, LogIfTooSlow]
            public void CallsBatchUpdateWithIsDeletedStatus()
            {
                setupTest();

                testedInteractor.Execute().Wait();

                dataSource.Received().BatchUpdate(
                    Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(entries => entries.All(entry => entry.IsDeleted))
                );
            }

            [Fact, LogIfTooSlow]
            public void InitiatesPushSync()
            {
                setupTest();

                testedInteractor.Execute().Wait();

                SyncManager.Received().PushSync();
            }
        }
    }
}
