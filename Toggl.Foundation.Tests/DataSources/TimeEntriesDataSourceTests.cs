using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.Models;
using Toggl.Foundation.DataSources;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Exceptions;
using Toggl.PrimeRadiant.Models;
using Xunit;
using ThreadingTask = System.Threading.Tasks.Task;
using FoundationTimeEntry = Toggl.Foundation.Models.TimeEntry;
using UltrawaveTimeEntry = Toggl.Ultrawave.Models.TimeEntry;

namespace Toggl.Foundation.Tests.DataSources
{
    public class TimeEntriesDataSourceTests
    {
        public class TimeEntryDataSourceTest
        {
            protected ITimeEntriesSource TimeEntriesSource { get; }

            protected string ValidDescription { get; } = "Testing software";

            protected DateTimeOffset ValidTime { get; } = DateTimeOffset.Now;

            protected IIdProvider IdProvider { get; } = Substitute.For<IIdProvider>();

            protected IDatabaseTimeEntry TimeEntry { get; } = FoundationTimeEntry.Clean(new UltrawaveTimeEntry { Id = 13 });

            protected IRepository<IDatabaseTimeEntry> Repository { get; } = Substitute.For<IRepository<IDatabaseTimeEntry>>();

            public TimeEntryDataSourceTest()
            {
                TimeEntriesSource = new TimeEntriesDataSource(IdProvider, Repository);

                IdProvider.GetNextIdentifier().Returns(-1);
                Repository.GetById(Arg.Is(TimeEntry.Id)).Returns(Observable.Return(TimeEntry));
                Repository.Create(Arg.Any<IDatabaseTimeEntry>())
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
                var observer = new TestScheduler().CreateObserver<ITimeEntry>();
                TimeEntriesSource.CurrentlyRunningTimeEntry.Where(te => te != null).Subscribe(observer);

                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                var currentlyRunningTimeEntry = observer.Messages.Single().Value.Value;
                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te .Start == currentlyRunningTimeEntry.Start));
            }
        }

        public class TheStopMethod : TimeEntryDataSourceTest
        {
            public TheStopMethod()
            {
                var timeEntries = new List<IDatabaseTimeEntry>
                {
                    TimeEntry,
                    TimeEntry.With(DateTimeOffset.UtcNow),
                    TimeEntry.With(DateTimeOffset.UtcNow),
                    TimeEntry.With(DateTimeOffset.UtcNow)
                };

                Repository
                    .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo =>
                        Observable
                             .Return(timeEntries)
                             .Select(x => x.Where(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));
            }
             
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
                var observer = new TestScheduler().CreateObserver<ITimeEntry>();
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
                    TimeEntry.With(DateTimeOffset.UtcNow),
                    TimeEntry.With(DateTimeOffset.UtcNow),
                    TimeEntry.With(DateTimeOffset.UtcNow)
                };

                Repository
                    .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo =>
                        Observable
                             .Return(timeEntries)
                             .Select(x => x.Where(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));

                var observer = new TestScheduler().CreateObserver<ITimeEntry>();
                var observable = TimeEntriesSource.Stop(ValidTime);
                observable.Subscribe(observer);

                observer.Messages.Single().Value.Exception.Should().BeOfType<InvalidOperationException>();
            }
        } 

        public class TheDeleteMethod : TimeEntryDataSourceTest
        {
            [Fact]
            public async ThreadingTask SetsTheDeletedFlag()
            {
                await TimeEntriesSource.Delete(TimeEntry.Id).LastOrDefaultAsync();

                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDeleted == true));
            }

            [Fact]
            public async ThreadingTask SetsTheDirtyFlag()
            {
                await TimeEntriesSource.Delete(TimeEntry.Id).LastOrDefaultAsync();
               
                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty == true));
            }

            [Fact]
            public async ThreadingTask UpdatesTheCorrectTimeEntry()
            {
                await TimeEntriesSource.Delete(TimeEntry.Id).LastOrDefaultAsync();

                await Repository.Received().GetById(Arg.Is(TimeEntry.Id));
                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.Id == TimeEntry.Id));
            }

            [Fact]
            public void DoesNotEmitAnyElements()
            {
                var observer = Substitute.For<IObserver<Unit>>();

                TimeEntriesSource.Delete(TimeEntry.Id).Subscribe(observer);

                observer.DidNotReceive().OnNext(Arg.Any<Unit>());
                observer.Received().OnCompleted();
            }

            [Fact]
            public void PropagatesErrorIfUpdateFails()
            {
                var timeEntry = FoundationTimeEntry.Clean(new UltrawaveTimeEntry { Id = 12 });
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
