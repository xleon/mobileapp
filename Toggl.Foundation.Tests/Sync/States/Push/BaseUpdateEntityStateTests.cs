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
            private IRepository<TModel> repository;

            public TheStartMethod()
            {
                this.api = Substitute.For<ITogglApi>();
                this.repository = Substitute.For<IRepository<TModel>>();
            }

            public void ReturnsTheFailTransitionWhenEntityIsNull()
            {
                var state = CreateState(api, repository);
                var transition = state.Start(null).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.UpdatingFailed);
                parameter.Reason.Should().BeOfType<ArgumentNullException>();
            }

            public void ReturnsTheFailTransitionWhenHttpFails()
            {
                var state = CreateState(api, repository);
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
                var state = CreateState(api, repository);
                var entity = CreateDirtyEntity(1);
                repository
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(_ => Observable.Throw<IEnumerable<(ConflictResolutionMode, TModel)>>(new TestException()));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.UpdatingFailed);
                parameter.Reason.Should().BeOfType<TestException>();
            }

            public void UpdateApiCallIsCalledWithTheInputEntity()
            {
                var state = CreateState(api, repository);
                var entity = CreateDirtyEntity(1);
                GetUpdateFunction(api)(entity)
                    .Returns(Observable.Return(Substitute.For<TApiModel>()));
                repository
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(Observable.Return(new[] { (ConflictResolutionMode.Update, entity) }));

                state.Start(entity).SingleAsync().Wait();

                AssertUpdateReceived(api, entity);
            }

            public void ReturnsTheEntityChangedTransitionWhenEntityChangesLocally()
            {
                var state = CreateState(api, repository);
                var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
                var entity = CreateDirtyEntity(1, at);
                GetUpdateFunction(api)(Arg.Any<TModel>())
                    .Returns(Observable.Return(entity));
                repository
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(Observable.Return(new[] { (ConflictResolutionMode.Ignore, entity) }));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<TModel>)transition).Parameter;

                transition.Result.Should().Be(state.EntityChanged);
                parameter.Id.Should().Be(entity.Id);
            }

            public void ReturnsTheUpdatingSuccessfulTransitionWhenEntityDoesNotChangeLocallyAndAllFunctionsAreCalledWithCorrectParameters()
            {
                var state = CreateState(api, repository);
                var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
                var entity = CreateDirtyEntity(1, at);
                var serverEntity = CreateDirtyEntity(2, at);
                var localEntity = CreateDirtyEntity(3, at);
                var updatedEntity = CreateDirtyEntity(4, at);
                GetUpdateFunction(api)(entity)
                    .Returns(Observable.Return(serverEntity));
                repository
                    .GetById(entity.Id)
                    .Returns(Observable.Return(localEntity));
                repository
                    .BatchUpdate(Arg.Any<IEnumerable<(long, TModel)>>(), Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>())
                    .Returns(Observable.Return(new[] { (ConflictResolutionMode.Update, updatedEntity) }));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<TModel>)transition).Parameter;

                transition.Result.Should().Be(state.UpdatingSucceeded);
                parameter.ShouldBeEquivalentTo(updatedEntity, options => options.IncludingProperties());
                repository.Received().BatchUpdate(
                    Arg.Is<IEnumerable<(long Id, TModel Entity)>>(
                        x => x.First().Id == entity.Id && x.First().Entity.Id == serverEntity.Id),
                    Arg.Any<Func<TModel, TModel, ConflictResolutionMode>>());
            }

            protected abstract BaseUpdateEntityState<TModel> CreateState(ITogglApi api, IRepository<TModel> repository);

            protected abstract Func<TModel, IObservable<TApiModel>> GetUpdateFunction(ITogglApi api);

            protected abstract TModel CreateDirtyEntity(long id, DateTimeOffset lastUpdate = default(DateTimeOffset));

            protected abstract void AssertUpdateReceived(ITogglApi api, TModel entity);
        }
    }
}
