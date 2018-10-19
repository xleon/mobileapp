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
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States.CleanUp
{
    public sealed class DeleteNonReferencedInaccessibleTagsStateTests : DeleteNonReferencedInaccessibleEntityTests
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
            var accessibleWorkspace = GetWorkspace(1000, isInaccessible: false);
            var inaccessibleWorkspace = GetWorkspace(2000, isInaccessible: true);

            var tag1 = GetTag(1001, accessibleWorkspace, SyncStatus.InSync);
            var tag2 = GetTag(1002, accessibleWorkspace, SyncStatus.SyncFailed);
            var tag3 = GetTag(1003, accessibleWorkspace, SyncStatus.RefetchingNeeded);
            var tag4 = GetTag(2001, inaccessibleWorkspace, SyncStatus.InSync);
            var tag5 = GetTag(2002, inaccessibleWorkspace, SyncStatus.SyncNeeded);
            var tag6 = GetTag(2003, inaccessibleWorkspace, SyncStatus.RefetchingNeeded);
            var tag7 = GetTag(2004, inaccessibleWorkspace, SyncStatus.InSync);
            var tag8 = GetTag(2005, inaccessibleWorkspace, SyncStatus.InSync);

            var te1 = GetTimeEntry(10001, accessibleWorkspace, SyncStatus.InSync, new[] { tag1 });
            var te2 = GetTimeEntry(10002, accessibleWorkspace, SyncStatus.SyncNeeded, new[] { tag2 });
            var te3 = GetTimeEntry(20001, inaccessibleWorkspace, SyncStatus.InSync, new[] { tag4 });
            var te4 = GetTimeEntry(20002, inaccessibleWorkspace, SyncStatus.SyncNeeded, new[] { tag5 });

            var tags = new[] { tag1, tag2, tag3, tag4, tag5, tag6, tag7, tag8 };
            var timeEntries = new[] { te1, te2, te3, te4 };

            var unreferencedTags = new[] { tag7, tag8 };
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
    }
}
