using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Interactors.TimeEntry
{
    public class ObserveTimeEntriesChangesInteractorTests : BaseInteractorTests
    {
        [Fact, LogIfTooSlow]
        public async Task GetsAnEventWhenAChangeToTimeEntriesHappens()
        {
            var createSubject = new Subject<IThreadSafeTimeEntry>();
            var deleteSubject = new Subject<long>();
            DataSource.TimeEntries.Created.Returns(createSubject.AsObservable());
            DataSource.TimeEntries.Updated.Returns(Observable.Never<EntityUpdate<IThreadSafeTimeEntry>>());
            DataSource.TimeEntries.Deleted.Returns(deleteSubject.AsObservable());

            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<Unit>();

            InteractorFactory.ObserveTimeEntriesChanges().Execute()
                .Subscribe(observer);

            var mockTimeEntry = new MockTimeEntry { Id = 42 };
            createSubject.OnNext(mockTimeEntry);
            deleteSubject.OnNext(3);

            observer.Messages.Should().HaveCount(2);
        }
    }
}
