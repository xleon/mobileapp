using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Models;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Exceptions;
using Toggl.PrimeRadiant.Models;
using Xunit;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Foundation.Tests.DataSources
{
    public class TimeEntriesDataSourceTests
    {
        public class TimeEntryDataSourceTest
        {
            protected const long CurrentRunningId = 13; 
            
            protected ITimeEntriesSource TimeEntriesSource { get; }

            protected TestScheduler TestScheduler { get; } = new TestScheduler();

            protected string ValidDescription { get; } = "Testing software";

            protected DateTimeOffset ValidTime { get; } = DateTimeOffset.Now;

            protected IIdProvider IdProvider { get; } = Substitute.For<IIdProvider>();

            protected IDatabaseTimeEntry DatabaseTimeEntry { get; } = 
                TimeEntry.Builder
                      .Create(CurrentRunningId)
                      .SetStart(DateTimeOffset.Now.AddHours(-2))
                      .SetIsDirty(false)
                      .SetDescription("")
                      .Build();

            protected IRepository<IDatabaseTimeEntry> Repository { get; } = Substitute.For<IRepository<IDatabaseTimeEntry>>();

            public TimeEntryDataSourceTest()
            {
                TimeEntriesSource = new TimeEntriesDataSource(IdProvider, Repository);

                IdProvider.GetNextIdentifier().Returns(-1);
                Repository.GetById(Arg.Is(DatabaseTimeEntry.Id)).Returns(Observable.Return(DatabaseTimeEntry));

                Repository.Create(Arg.Any<IDatabaseTimeEntry>())
                          .Returns(info => Observable.Return(info.Arg<IDatabaseTimeEntry>()));

                Repository.Update(Arg.Any<IDatabaseTimeEntry>())
                          .Returns(info => Observable.Return(info.Arg<IDatabaseTimeEntry>()));
            }
        }

        public class TheStartMethod : TimeEntryDataSourceTest
        {
            [Fact]
            public async ThreadingTask CreatesANewTimeEntryInTheDatabase()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Any<IDatabaseTimeEntry>());
            }

            [Fact]
            public async ThreadingTask CreatesADirtyTimeEntry()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty));
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async ThreadingTask CreatesATimeEntryWithTheProvidedValueForBillable(bool billable)
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, billable);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Billable == billable));
            }

            [Fact]
            public async ThreadingTask CreatesATimeEntryWithTheProvidedValueForDescription()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Description == ValidDescription));
            }

            [Fact]
            public async ThreadingTask CreatesATimeEntryWithTheProvidedValueForStartTime()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Start == ValidTime));
            }

            [Fact]
            public async ThreadingTask CreatesATimeEntryWithAnIdProvidedByTheIdProvider()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Id == -1));
            }

            [Fact]
            public async ThreadingTask SetstheCreatedTimeEntryAsTheCurrentlyRunningTimeEntry()
            {
                var observer = TestScheduler.CreateObserver<ITimeEntry>();
                TimeEntriesSource.CurrentlyRunningTimeEntry.Where(te => te != null).Subscribe(observer);

                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                var currentlyRunningTimeEntry = observer.Messages.Single().Value.Value;
                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Start == currentlyRunningTimeEntry.Start));
            }

            [Fact]
            public async ThreadingTask EmitsANewEventOnTheTimeEntryCreatedObservable()
            {
                var observer = TestScheduler.CreateObserver<ITimeEntry>();
                TimeEntriesSource.TimeEntryCreated.Subscribe(observer);

                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                observer.Messages.Single().Value.Value.Id.Should().Be(-1);
                observer.Messages.Single().Value.Value.Start.Should().Be(ValidTime);
            }
        }

        public sealed class TheGetAllMethod : TimeEntryDataSourceTest
        {
            [Fact]
            public async ThreadingTask NeverReturnsDeletedTimeEntries()
            {
                var result = Enumerable
                    .Range(0, 20)
                    .Select(i => 
                    {
                        var isDeleted = i % 2 == 0;
                        var timeEntry = Substitute.For<IDatabaseTimeEntry>();
                        timeEntry.Id.Returns(i);
                        timeEntry.IsDeleted.Returns(isDeleted);
                        return timeEntry;
                    });
                Repository
                    .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo =>
                        Observable
                             .Return(result)
                             .Select(x => x.Where(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));

                var timeEntries = await TimeEntriesSource.GetAll(x => x.Id > 10);

                timeEntries.Should().HaveCount(5);
            }
        }

        public class TheStopMethod : TimeEntryDataSourceTest
        {
            public TheStopMethod()
            {
                var timeEntries = new List<IDatabaseTimeEntry>
                {
                    DatabaseTimeEntry,
                    DatabaseTimeEntry.With(DateTimeOffset.UtcNow),
                    DatabaseTimeEntry.With(DateTimeOffset.UtcNow),
                    DatabaseTimeEntry.With(DateTimeOffset.UtcNow)
                };

                Repository
                    .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo =>
                        Observable
                             .Return(timeEntries)
                             .Select(x => x.Where(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));
            }

            [Fact]
            public async ThreadingTask UpdatesTheTimeEntrySettingItsStopTime()
            {
                await TimeEntriesSource.Stop(ValidTime); 
                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.Stop == ValidTime));
            }

            [Fact]
            public async ThreadingTask UpdatesTheTimeEntryMakingItDirty()
            {
                await TimeEntriesSource.Stop(ValidTime);

                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty));
            }

            [Fact]
            public async ThreadingTask SetsTheCurrentlyRunningTimeEntryToNull()
            {
                var observer = TestScheduler.CreateObserver<ITimeEntry>();
                TimeEntriesSource.CurrentlyRunningTimeEntry.Subscribe(observer);
                Repository.Update(Arg.Any<IDatabaseTimeEntry>()).Returns(callInfo => Observable.Return(callInfo.Arg<IDatabaseTimeEntry>()));

                await TimeEntriesSource.Stop(ValidTime);

                observer.Messages.Single().Value.Value.Should().BeNull();
            }

            [Fact]
            public void ThrowsIfThereAreNoRunningTimeEntries()
            {
                var timeEntries = new List<IDatabaseTimeEntry>
                {
                    DatabaseTimeEntry.With(DateTimeOffset.UtcNow),
                    DatabaseTimeEntry.With(DateTimeOffset.UtcNow),
                    DatabaseTimeEntry.With(DateTimeOffset.UtcNow)
                };

                Repository
                    .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo =>
                        Observable
                             .Return(timeEntries)
                             .Select(x => x.Where(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));

                var observer = TestScheduler.CreateObserver<ITimeEntry>();
                var observable = TimeEntriesSource.Stop(ValidTime);
                observable.Subscribe(observer);

                observer.Messages.Single().Value.Exception.Should().BeOfType<InvalidOperationException>();
            }

            [Fact]
            public async ThreadingTask EmitsANewEventOnTheTimeEntryUpdatedObservable()
            {
                var observer = TestScheduler.CreateObserver<ITimeEntry>();
                TimeEntriesSource.TimeEntryUpdated.Subscribe(observer);

                await TimeEntriesSource.Stop(ValidTime);
               
                observer.Messages.Single().Value.Value.Id.Should().Be(CurrentRunningId);
                observer.Messages.Single().Value.Value.Stop.Should().Be(ValidTime);
            }
        } 
        public class TheDeleteMethod : TimeEntryDataSourceTest
        {
            [Fact]
            public async ThreadingTask SetsTheDeletedFlag()
            {
                await TimeEntriesSource.Delete(DatabaseTimeEntry.Id).LastOrDefaultAsync();

                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDeleted == true));
            }

            [Fact]
            public async ThreadingTask SetsTheDirtyFlag()
            {
                await TimeEntriesSource.Delete(DatabaseTimeEntry.Id).LastOrDefaultAsync();
               
                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty == true));
            }

            [Fact]
            public async ThreadingTask UpdatesTheCorrectTimeEntry()
            {
                await TimeEntriesSource.Delete(DatabaseTimeEntry.Id).LastOrDefaultAsync();

                await Repository.Received().GetById(Arg.Is(DatabaseTimeEntry.Id));
                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.Id == DatabaseTimeEntry.Id));
            }

            [Fact]
            public void DoesNotEmitAnyElements()
            {
                var observer = Substitute.For<IObserver<Unit>>();

                TimeEntriesSource.Delete(DatabaseTimeEntry.Id).Subscribe(observer);

                observer.DidNotReceive().OnNext(Arg.Any<Unit>());
                observer.Received().OnCompleted();
            }

            [Fact]
            public void PropagatesErrorIfUpdateFails()
            {
                var timeEntry = TimeEntry.Builder.Create(12)
                      .SetStart(DateTimeOffset.Now)
                      .SetIsDirty(false)
                      .SetDescription("")
                      .Build();
                
                var timeEntryObservable = Observable.Return(timeEntry);
                var errorObservable = Observable.Throw<IDatabaseTimeEntry>(new EntityNotFoundException());
                Repository.GetById(Arg.Is(timeEntry.Id)).Returns(timeEntryObservable);
                Repository.Update(Arg.Any<IDatabaseTimeEntry>()).Returns(errorObservable);
                var observer = Substitute.For<IObserver<Unit>>();

                TimeEntriesSource.Delete(timeEntry.Id).Subscribe(observer);

                observer.Received().OnError(Arg.Any<EntityNotFoundException>());
            }

            [Fact]
            public void PropagatesErrorIfTimeEntryIsNotInRepository()
            {
                var observer = Substitute.For<IObserver<Unit>>();
                Repository.GetById(Arg.Any<long>())
                          .Returns(Observable.Throw<IDatabaseTimeEntry>(new EntityNotFoundException()));

                TimeEntriesSource.Delete(12).Subscribe(observer);

                observer.Received().OnError(Arg.Any<EntityNotFoundException>());
            }
        }
    }
}
