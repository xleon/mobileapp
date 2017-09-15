using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States
{
    public abstract class BaseUpdateEntityStateTests
    {
        private TheStartMethodHelper helper;

        public BaseUpdateEntityStateTests(TheStartMethodHelper helper)
        {
            this.helper = helper;
        }

        [Fact]
        public void ReturnsTheFailTransitionWhenEntityIsNull()
            => helper.ReturnsTheFailTransitionWhenEntityIsNull();

        [Fact]
        public void ReturnsTheFailTransitionWhenHttpFails()
            => helper.ReturnsTheFailTransitionWhenHttpFails();

        [Fact]
        public void ReturnsTheFailTransitionWhenDatabaseOperationFails()
            => helper.ReturnsTheFailTransitionWhenDatabaseOperationFails();

        [Fact]
        public void UpdateApiCallIsCalledWithTheInputEntity()
            => helper.UpdateApiCallIsCalledWithTheInputEntity();

        [Fact]
        public void ReturnsTheEntityChangedTransitionWhenEntityChangesLocally()
            => helper.ReturnsTheEntityChangedTransitionWhenEntityChangesLocally();

        [Fact]
        public void ReturnsTheUpdatingSuccessfulTransitionWhenIfEntityChangesLocallyAndAllFunctionsAreCalledWithCorrectParameters()
            => helper.ReturnsTheUpdatingSuccessfulTransitionWhenEntityDoesNotChangeLocallyAndAllFunctionsAreCalledWithCorrectParameters();

        public interface TheStartMethodHelper
        {
            void ReturnsTheFailTransitionWhenEntityIsNull();
            void ReturnsTheFailTransitionWhenHttpFails();
            void ReturnsTheFailTransitionWhenDatabaseOperationFails();
            void UpdateApiCallIsCalledWithTheInputEntity();
            void ReturnsTheEntityChangedTransitionWhenEntityChangesLocally();
            void ReturnsTheUpdatingSuccessfulTransitionWhenEntityDoesNotChangeLocallyAndAllFunctionsAreCalledWithCorrectParameters();
        }

        internal abstract class TheStartMethod<TModel, TApiModel> : TheStartMethodHelper
            where TModel : class, IBaseModel, IDatabaseSyncable, TApiModel
            where TApiModel : class
        {
            private ITogglApi api;
            private ITogglDatabase database;

            public TheStartMethod()
            {
                this.api = Substitute.For<ITogglApi>();
                this.database = Substitute.For<ITogglDatabase>();
            }

            public void ReturnsTheFailTransitionWhenEntityIsNull()
            {
                var state = CreateState(api, database);
                var transition = state.Start(null).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.UpdatingFailed);
                parameter.Reason.Should().BeOfType<ArgumentNullException>();
            }

            public void ReturnsTheFailTransitionWhenHttpFails()
            {
                var state = CreateState(api, database);
                var entity = CreateDirtyEntity(1);
                GetUpdateFunction(api)(Arg.Any<TModel>())
                    .Returns(_ => Observable.Throw<TApiModel>(new TestException()));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.UpdatingFailed);
                parameter.Reason.Should().BeOfType<TestException>();
            }

            public void ReturnsTheFailTransitionWhenDatabaseOperationFails()
            {
                var state = CreateState(api, database);
                var entity = CreateDirtyEntity(1);
                GetRepository(database)
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(_ => Observable.Throw<IEnumerable<(ConflictResolutionMode, TModel)>>(new TestException()));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.UpdatingFailed);
                parameter.Reason.Should().BeOfType<TestException>();
            }

            public void UpdateApiCallIsCalledWithTheInputEntity()
            {
                var state = CreateState(api, database);
                var entity = CreateDirtyEntity(1);
                GetUpdateFunction(api)(entity)
                    .Returns(Observable.Return(Substitute.For<TApiModel>()));
                GetRepository(database)
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(Observable.Return(new[] { (ConflictResolutionMode.Update, entity) }));

                state.Start(entity).SingleAsync().Wait();

                AssertUpdateReceived(api, entity);
            }

            public void ReturnsTheEntityChangedTransitionWhenEntityChangesLocally()
            {
                var state = CreateState(api, database);
                var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
                var entity = CreateDirtyEntity(1, at);
                GetUpdateFunction(api)(Arg.Any<TModel>())
                    .Returns(Observable.Return(entity));
                GetRepository(database)
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(Observable.Return(new[] { (ConflictResolutionMode.Ignore, entity) }));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<TModel>)transition).Parameter;

                transition.Result.Should().Be(state.EntityChanged);
                parameter.Id.Should().Be(entity.Id);
            }

            public void ReturnsTheUpdatingSuccessfulTransitionWhenEntityDoesNotChangeLocallyAndAllFunctionsAreCalledWithCorrectParameters()
            {
                var state = CreateState(api, database);
                var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
                var entity = CreateDirtyEntity(1, at);
                var serverEntity = CreateDirtyEntity(2, at);
                var localEntity = CreateDirtyEntity(3, at);
                var updatedEntity = CreateDirtyEntity(4, at);
                GetUpdateFunction(api)(entity)
                    .Returns(Observable.Return(serverEntity));
                GetRepository(database)
                    .GetById(entity.Id)
                    .Returns(Observable.Return(localEntity));
                GetRepository(database)
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(Observable.Return(new[] { (ConflictResolutionMode.Update, updatedEntity) }));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<TModel>)transition).Parameter;

                transition.Result.Should().Be(state.UpdatingSucceeded);
                parameter.ShouldBeEquivalentTo(updatedEntity, options => options.IncludingProperties());
                GetRepository(database).Received().BatchUpdate(
                    Arg.Is<IEnumerable<(long Id, TModel Entity)>>(
                        x => x.First().Id == entity.Id && x.First().Entity.Id == serverEntity.Id),
                    Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>());
            }

            protected abstract BaseUpdateEntityState<TModel> CreateState(ITogglApi api, ITogglDatabase database);

            protected abstract IRepository<TModel> GetRepository(ITogglDatabase database);

            protected abstract Func<TModel, IObservable<TApiModel>> GetUpdateFunction(ITogglApi api);

            protected abstract TModel CreateDirtyEntity(long id, DateTimeOffset lastUpdate = default(DateTimeOffset));

            protected abstract void AssertUpdateReceived(ITogglApi api, TModel entity);
        }
    }
}
