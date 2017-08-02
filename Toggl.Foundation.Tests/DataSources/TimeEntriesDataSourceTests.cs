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
        public class TheDeleteMethod
        {
            private (IRepository<IDatabaseTimeEntry>, TimeEntriesDataSource, ITimeEntry) setup()
            {
                var repository = Substitute.For<IRepository<IDatabaseTimeEntry>>();
                var timeEntry = FoundationTimeEntry.Clean(new UltrawaveTimeEntry { Id = 13 });
                var observable = Observable.Return(timeEntry);
                repository.GetById(Arg.Is(timeEntry.Id)).Returns(observable);
                var dataSource = new TimeEntriesDataSource(repository);
                return (repository, dataSource, timeEntry);
            }

            [Fact]
            public async Task SetsTheDeletedFlag()
            {
                var (repository, dataSource, timeEntry) = setup();

                await dataSource.Delete(timeEntry.Id).LastOrDefaultAsync();

                await repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDeleted == true));
            }

            [Fact]
            public async Task SetsTheDirtyFlag()
            {
                var (repository, dataSource, timeEntry) = setup();

                await dataSource.Delete(timeEntry.Id).LastOrDefaultAsync();
               
                await repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty == true));
            }

            [Fact]
            public async Task UpdatesTheCorrectTimeEntry()
            {
                var (repository, dataSource, timeEntry) = setup();

                await dataSource.Delete(timeEntry.Id).LastOrDefaultAsync();

                await repository.Received().GetById(Arg.Is(timeEntry.Id));
                await repository.Received().Update(Arg.Is<IDatabaseTimeEntry>(te => te.Id == timeEntry.Id));
            }

            [Fact]
            public void DoesNotEmitAnyElements()
            {
                var (repository, dataSource, timeEntry) = setup();
                var observer = Substitute.For<IObserver<Unit>>();

                dataSource.Delete(timeEntry.Id).Subscribe(observer);

                observer.DidNotReceive().OnNext(Arg.Any<Unit>());
                observer.Received().OnCompleted();
            }

            [Fact]
            public void PropagatesErrorIfUpdateFails()
            {
                var repository = Substitute.For<IRepository<IDatabaseTimeEntry>>();
                var timeEntry = FoundationTimeEntry.Clean(new UltrawaveTimeEntry { Id = 12 });
                var timeEntryObservable = Observable.Return(timeEntry);
                var errorObservable = Observable.Throw<IDatabaseTimeEntry>(new EntityNotFoundException());
                repository.GetById(Arg.Is(timeEntry.Id)).Returns(timeEntryObservable);
                repository.Update(Arg.Any<IDatabaseTimeEntry>()).Returns(errorObservable);
                var dataSource = new TimeEntriesDataSource(repository);
                var observer = Substitute.For<IObserver<Unit>>();

                dataSource.Delete(timeEntry.Id).Subscribe(observer);

                observer.Received().OnError(Arg.Any<EntityNotFoundException>());
            }

            [Fact]
            public void PropagatesErrorIfTimeEntryIsNotInRepository()
            {
                var repository = Substitute.For<IRepository<IDatabaseTimeEntry>>();
                var dataSource = new TimeEntriesDataSource(repository);
                var observer = Substitute.For<IObserver<Unit>>();
                repository.GetById(Arg.Any<int>())
                          .Returns(Observable.Throw<IDatabaseTimeEntry>(new EntityNotFoundException()));

                dataSource.Delete(12).Subscribe(observer);

                observer.Received().OnError(Arg.Any<EntityNotFoundException>());
            }
        }
    }
}
