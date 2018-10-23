using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.States.CleanUp;
using Toggl.Foundation.Tests.Mocks;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States.CleanUp
{
    public class DeleteInaccessibleTimeEntriesStateTests
    {
        private readonly ITimeEntriesSource dataSource = Substitute.For<ITimeEntriesSource>();

        private readonly DeleteInaccessibleTimeEntriesState state;

        public DeleteInaccessibleTimeEntriesStateTests()
        {
            state = new DeleteInaccessibleTimeEntriesState(dataSource);
        }

        [Fact, LogIfTooSlow]
        public async Task DeletesSyncedInaccessibleTimeEntries()
        {
            var workspace = getWorkspace(1000, isInaccessible: true);
            var syncedTimeEntries = getSyncedTimeEntries(workspace);
            var unsyncedTimeEntries = getUnsyncedTimeEntries(workspace);
            var allTimeEntries = syncedTimeEntries.Concat(unsyncedTimeEntries);

            configureDataSource(allTimeEntries);

            await state.Start().SingleAsync();

            dataSource
                .Received()
                .DeleteAll(Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(
                    arg => arg.All(te => syncedTimeEntries.Contains(te)) &&
                           arg.None(te => unsyncedTimeEntries.Contains(te))));
        }

        [Fact, LogIfTooSlow]
        public async Task OnlyDeletesSyncedInaccessibleTimeEntries()
        {
            var workspaceA = getWorkspace(1000, isInaccessible: true);
            var workspaceB = getWorkspace(2000, isInaccessible: true);
            var workspaceC = getWorkspace(3000, isInaccessible: false);

            var syncedInaccessibleTimeEntries = getSyncedTimeEntries(workspaceA)
                .Concat(getSyncedTimeEntries(workspaceB));

            var undeletableTimeEntries = getUnsyncedTimeEntries(workspaceA)
                .Concat(getUnsyncedTimeEntries(workspaceB))
                .Concat(getSyncedTimeEntries(workspaceC))
                .Concat(getUnsyncedTimeEntries(workspaceC));

            var allTimeEntries = syncedInaccessibleTimeEntries.Concat(undeletableTimeEntries);

            configureDataSource(allTimeEntries);

            await state.Start().SingleAsync();

            dataSource
                .Received()
                .DeleteAll(Arg.Is<IEnumerable<IThreadSafeTimeEntry>>(
                    arg => arg.All(te => syncedInaccessibleTimeEntries.Contains(te)) &&
                           arg.None(te => undeletableTimeEntries.Contains(te))));
        }

        private void configureDataSource(IEnumerable<IThreadSafeTimeEntry> timeEntries)
        {
            dataSource
                .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>(), Arg.Is(true))
                .Returns(callInfo =>
                {
                    var predicate = callInfo[0] as Func<IDatabaseTimeEntry, bool>;
                    var filteredTimeEntries = timeEntries.Where(predicate);
                    return Observable.Return(filteredTimeEntries.Cast<IThreadSafeTimeEntry>());
                });
        }

        private IThreadSafeWorkspace getWorkspace(long id, bool isInaccessible)
            => new MockWorkspace
            {
                Id = id,
                Name = "Some workspace",
                IsInaccessible = isInaccessible
            };

        private List<IThreadSafeTimeEntry> getSyncedTimeEntries(IThreadSafeWorkspace workspace)
            => new List<IThreadSafeTimeEntry>
                {
                    new MockTimeEntry
                    {
                        Id = workspace.Id + 1,
                        Workspace = workspace,
                        WorkspaceId = workspace.Id,
                        SyncStatus = PrimeRadiant.SyncStatus.InSync
                    },
                    new MockTimeEntry
                    {
                        Id = workspace.Id + 2,
                        Workspace = workspace,
                        WorkspaceId = workspace.Id,
                        SyncStatus = PrimeRadiant.SyncStatus.InSync
                    },
                    new MockTimeEntry
                    {
                       Id = workspace.Id + 3,
                        Workspace = workspace,
                        WorkspaceId = workspace.Id,
                        SyncStatus = PrimeRadiant.SyncStatus.InSync
                    }
                };

        private List<IThreadSafeTimeEntry> getUnsyncedTimeEntries(IThreadSafeWorkspace workspace)
            => new List<IThreadSafeTimeEntry>
                {
                    new MockTimeEntry
                    {
                        Id = workspace.Id + 4,
                        Workspace = workspace,
                        WorkspaceId = workspace.Id,
                        SyncStatus = PrimeRadiant.SyncStatus.RefetchingNeeded
                    },
                    new MockTimeEntry
                    {
                        Id = workspace.Id + 5,
                        Workspace = workspace,
                        WorkspaceId = workspace.Id,
                        SyncStatus = PrimeRadiant.SyncStatus.SyncFailed
                    },
                    new MockTimeEntry
                    {
                        Id = workspace.Id + 6,
                        Workspace = workspace,
                        WorkspaceId = workspace.Id,
                        SyncStatus = PrimeRadiant.SyncStatus.SyncNeeded
                    },
                };
    }
}
