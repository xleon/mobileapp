using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac.Extensions;
using Xunit;

namespace Toggl.Foundation.Tests.Interactors.TimeEntry
{
    public class UpdateMultipleTimeEntriesInteractorTests : BaseInteractorTests
    {
        private IInteractorFactory interactorFactory = Substitute.For<IInteractorFactory>();
        private IInteractor<IObservable<IThreadSafeTimeEntry>> interactor = Substitute.For<IInteractor<IObservable<IThreadSafeTimeEntry>>>();
        private EditTimeEntryDto[] timeEntriesDtos;

        [Fact, LogIfTooSlow]
        public async Task CallsUpdateTimeEntryInteractorCorrectNumberOfTimes()
        {
            arrangeTest();

            await new UpdateMultipleTimeEntriesInteractor(interactorFactory, timeEntriesDtos).Execute();

            await interactor.Received(timeEntriesDtos.Length).Execute();
        }

        [Fact, LogIfTooSlow]
        public async Task CallsUpdateTimeEntryInteractorCorrectDtos()
        {
            arrangeTest();

            await new UpdateMultipleTimeEntriesInteractor(interactorFactory, timeEntriesDtos).Execute();

            foreach (var dto in timeEntriesDtos)
            {
                interactorFactory.Received().UpdateTimeEntry(dto);
            }
        }

        private void arrangeTest()
        {
            timeEntriesDtos = Enumerable
                .Range(0, 8)
                .Select(createDto)
                .ToArray();

            timeEntriesDtos.ForEach(dto => interactorFactory.UpdateTimeEntry(dto).Returns(interactor));
        }

        private EditTimeEntryDto createDto(int id)
        {
            return new EditTimeEntryDto
            {
                Id = id,
                Description = "Description",
                StartTime = DateTimeOffset.UtcNow,
                ProjectId = 13,
                Billable = true,
                WorkspaceId = 71,
                TagIds = new long[] { 12, 34, 56, 78 }
            };
        }
    }
}
