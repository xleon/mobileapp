using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.States.CleanUp;
using Toggl.Foundation.Tests.Mocks;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States.CleanUp
{
    public class DeleteNonReferencedInaccessibleTagsStateTests
    {
        private readonly DeleteNonReferencedInaccessibleTagsState state;

        private readonly ITimeEntriesSource timeEntriesDataSource = Substitute.For<ITimeEntriesSource>();

        private readonly IDataSource<IThreadSafeTag, IDatabaseTag> tagsDataSource = Substitute.For<IDataSource<IThreadSafeTag, IDatabaseTag>>();

        public DeleteNonReferencedInaccessibleTagsStateTests()
        {
            state = new DeleteNonReferencedInaccessibleTagsState(tagsDataSource, timeEntriesDataSource);
        }

        [Fact, LogIfTooSlow]
        public async Task DeleteUnreferencedTagsInInaccessibleWorkspace()
        {
            var accessibleWorkspace = getWorkspace(1000, isInaccessible: false);
            var inaccessibleWorkspace = getWorkspace(2000, isInaccessible: true);

            var tag1 = getTag(1001, accessibleWorkspace);
            var tag2 = getTag(1002, accessibleWorkspace);
            var tag3 = getTag(1003, accessibleWorkspace);
            var tag4 = getTag(2001, inaccessibleWorkspace);
            var tag5 = getTag(2002, inaccessibleWorkspace);
            var tag6 = getTag(2003, inaccessibleWorkspace);
            var tag7 = getTag(2004, inaccessibleWorkspace);

            var te1 = getTimeEntry(10001, accessibleWorkspace, new[] { tag1 }, SyncStatus.InSync);
            var te2 = getTimeEntry(10002, accessibleWorkspace, new[] { tag2 }, SyncStatus.SyncNeeded);
            var te3 = getTimeEntry(20001, inaccessibleWorkspace, new[] { tag4 }, SyncStatus.InSync);
            var te4 = getTimeEntry(20002, inaccessibleWorkspace, new[] { tag5 }, SyncStatus.SyncNeeded);

            var tags = new IThreadSafeTag[] { tag1, tag2, tag3, tag4, tag5, tag6, tag7 };
            var timeEntries = new IThreadSafeTimeEntry[] { te1, te2, te3, te4 };

            var unreferencedTags = new IThreadSafeTag[] { tag6, tag7 };
            var neededTags = tags.Where(tag => !unreferencedTags.Contains(tag));

            configureDataSource(tags, timeEntries);

            await state.Start().SingleAsync();

            tagsDataSource.Received().DeleteAll(Arg.Is<IEnumerable<IThreadSafeTag>>(arg =>
                arg.All(tag => unreferencedTags.Contains(tag)) &&
                arg.None(tag => neededTags.Contains(tag))));
        }

        private void configureDataSource(IThreadSafeTag[] tags, IThreadSafeTimeEntry[] timeEntries)
        {
            tagsDataSource
                .GetAll(Arg.Any<Func<IDatabaseTag, bool>>(), Arg.Is(true))
                .Returns(callInfo =>
                {
                    var predicate = callInfo[0] as Func<IDatabaseTag, bool>;
                    var filteredTags = tags.Where(predicate);
                    return Observable.Return(filteredTags.Cast<IThreadSafeTag>());
                });

            timeEntriesDataSource
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
                    IsInaccessible = isInaccessible
                };

        private IThreadSafeTag getTag(long id, IThreadSafeWorkspace workspace)
            => new MockTag
                {
                    Id = id,
                    Workspace = workspace,
                    WorkspaceId = workspace.Id
                };

        private IThreadSafeTimeEntry getTimeEntry(long id, IThreadSafeWorkspace workspace, IThreadSafeTag[] tags, SyncStatus syncStatus)
            => new MockTimeEntry
                {
                    Id = id,
                    Workspace = workspace,
                    WorkspaceId = workspace.Id,
                    Tags = tags,
                    TagIds = tags.Select(tag => tag.Id),
                    SyncStatus = syncStatus
                };
    }
}
