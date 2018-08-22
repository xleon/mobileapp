using System;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using System.Reactive.Linq;
using Toggl.Foundation.Extensions;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync;

namespace Toggl.Foundation.Tests.Interactors.TimeEntry
{
    public sealed class UpdateTimeEntryInteractorTests
    {
        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private DTOs.EditTimeEntryDto dto = new DTOs.EditTimeEntryDto
                {
                    Id = 8,
                    StartTime = new DateTimeOffset(2018, 8, 20, 9, 0, 0, TimeSpan.Zero),
                    StopTime = new DateTimeOffset(208, 8, 20, 11, 0, 0, TimeSpan.Zero)
                };

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheTimeEntry()
            {
                await InteractorFactory.UpdateTimeEntry(dto).Execute();
                await DataSource.TimeEntries.Received().Update(dto);
            }

            [Fact, LogIfTooSlow]
            public async Task TriggersPushSync()
            {
                var syncManager = Substitute.For<ISyncManager>();
                DataSource.SyncManager.Returns(syncManager);

                await InteractorFactory.UpdateTimeEntry(dto).Execute();

                syncManager.Received().InitiatePushSync();
            }
        }
    }
}
