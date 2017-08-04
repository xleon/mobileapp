﻿﻿﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Exceptions;
using Toggl.PrimeRadiant.Models;
using Xunit;
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

            protected IDatabaseTimeEntry TimeEntry { get; } = FoundationTimeEntry.Clean(new UltrawaveTimeEntry { Id = 13 });

            protected IRepository<IDatabaseTimeEntry> Repository { get; } = Substitute.For<IRepository<IDatabaseTimeEntry>>();

            public TimeEntryDataSourceTest()
            {
                TimeEntriesSource = new TimeEntriesDataSource(Repository);
                Repository.GetById(Arg.Is(TimeEntry.Id)).Returns(Observable.Return(TimeEntry));
                Repository.Create(Arg.Any<IDatabaseTimeEntry>())
                          .Returns(info => Observable.Return(info.Arg<IDatabaseTimeEntry>()));
            }
        }

        public class TheStartMethod : TimeEntryDataSourceTest
        {
            [Fact]
            public async Task CreatesANewTimeEntryInTheDatabase()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Any<IDatabaseTimeEntry>());
            }

            [Fact]
            public async Task CreatesADirtyTimeEntry()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty));
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task CreatesATimeEntryWithTheProvidedValueForBillable(bool billable)
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, billable);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Billable == billable));
            }

            [Fact]
            public async Task CreatesATimeEntryWithTheProvidedValueForDescription()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Description == ValidDescription));
            }

            [Fact]
            public async Task CreatesATimeEntryWithTheProvidedValueForStartTime()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Start == ValidTime));
            }

            [Fact]
            public async Task SetstheCreatedTimeEntryAsTheCurrentlyRunningTimeEntry()
            {
                var observer = new TestScheduler().CreateObserver<ITimeEntry>();
                TimeEntriesSource.CurrentlyRunningTimeEntry.Where(te => te != null).Subscribe(observer);

                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                var currentlyRunningTimeEntry = observer.Messages.Single().Value.Value;
                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te == currentlyRunningTimeEntry));
            }
        }

        public class TheDeleteMethod : TimeEntryDataSourceTest
        {
            [Fact]
            public async Task SetsTheDeletedFlag()
            {
                await TimeEntriesSource.Delete(TimeEntry.Id).LastOrDefaultAsync();

                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDeleted == true));
            }

            [Fact]
            public async Task SetsTheDirtyFlag()
            {
                await TimeEntriesSource.Delete(TimeEntry.Id).LastOrDefaultAsync();
               
                await Repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty == true));
            }

            [Fact]
            public async Task UpdatesTheCorrectTimeEntry()
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
                Repository.GetById(Arg.Any<int>())
                          .Returns(Observable.Throw<IDatabaseTimeEntry>(new EntityNotFoundException()));

                TimeEntriesSource.Delete(12).Subscribe(observer);

                observer.Received().OnError(Arg.Any<EntityNotFoundException>());
            }
        }
    }
}
