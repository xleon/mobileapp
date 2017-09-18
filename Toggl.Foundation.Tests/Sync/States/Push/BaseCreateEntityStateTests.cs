using System;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Exceptions;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States
{
    public abstract class BaseCreateEntityStateTests
    {
        private IStartMethodTestHelper helper;

        public BaseCreateEntityStateTests(IStartMethodTestHelper helper)
        {
            this.helper = helper;
        }

        [Fact]
        public void ReturnsFailTransitionWhenEntityIsNull()
            => helper.ReturnsFailTransitionWhenEntityIsNull();

        [Fact]
        public void ReturnsFailTransitionWhenHttpFails()
            => helper.ReturnsFailTransitionWhenHttpFails();

        [Fact]
        public void ReturnsFailTransitionWhenDatabaseOperationFails()
            => helper.ReturnsFailTransitionWhenDatabaseOperationFails();

        [Fact]
        public void ReturnsSuccessfulTransitionWhenEverythingWorks()
            => helper.ReturnsSuccessfulTransitionWhenEverythingWorks();

        [Fact]
        public void UpdateIsCalledWithCorrectParameters()
            => helper.UpdateIsCalledWithCorrectParameters();

        public interface IStartMethodTestHelper
        {
            void ReturnsFailTransitionWhenEntityIsNull();
            void ReturnsFailTransitionWhenHttpFails();
            void ReturnsFailTransitionWhenDatabaseOperationFails();
            void ReturnsSuccessfulTransitionWhenEverythingWorks();
            void UpdateIsCalledWithCorrectParameters();
        }

        internal abstract class TheStartMethod<TModel, TApiModel> : IStartMethodTestHelper
            where TModel : class, IBaseModel, IDatabaseSyncable, TApiModel
        {
            private ITogglApi api;
            private IRepository<TModel> repository;

            public TheStartMethod()
            {
                api = Substitute.For<ITogglApi>();
                repository = Substitute.For<IRepository<TModel>>();
            }
            
            public void ReturnsFailTransitionWhenEntityIsNull()
            {
                var state = CreateState(api, repository);

                var transition = state.Start(null).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.CreatingFailed);
                parameter.Reason.Should().BeOfType<ArgumentNullException>();
            }

            public void ReturnsFailTransitionWhenHttpFails()
            {
                var state = CreateState(api, repository);
                var entity = CreateDirtyEntityWithNegativeId();
                GetCreateFunction(api)(Arg.Any<TModel>())
                    .Returns(_ => Observable.Throw<TModel>(new TestException()));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.CreatingFailed);
                parameter.Reason.Should().BeOfType<TestException>();
            }

            public void ReturnsFailTransitionWhenDatabaseOperationFails()
            {
                var state = CreateState(api, repository);
                var entity = CreateDirtyEntityWithNegativeId();
                repository.Update(Arg.Any<long>(), Arg.Any<TModel>())
                    .Returns(_ => Observable.Throw<TModel>(new TestException()));

                var transition = state.Start(entity).SingleAsync().Wait();
                var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

                transition.Result.Should().Be(state.CreatingFailed);
                parameter.Reason.Should().BeOfType<TestException>();
            }

            public void ReturnsSuccessfulTransitionWhenEverythingWorks()
            {
                var state = CreateState(api, repository);
                var entity = CreateDirtyEntityWithNegativeId();
                var clean = CreateCleanEntityFrom(entity);
                var withPositiveId = CreateCleanWithPositiveIdFrom(entity);
                GetCreateFunction(api)(Arg.Any<TModel>())
                    .Returns(Observable.Return(withPositiveId));
                repository.Update(Arg.Any<long>(), Arg.Any<TModel>())
                    .Returns(x => Observable.Return((TModel)x[1]));

                var transition = state.Start(entity).SingleAsync().Wait();
                var persistedEntity = ((Transition<TModel>)transition).Parameter;

                transition.Result.Should().Be(state.CreatingFinished);
                persistedEntity.Id.Should().NotBe(entity.Id);
                persistedEntity.Id.Should().BeGreaterThan(0);
                persistedEntity.SyncStatus.Should().Be(SyncStatus.InSync);
                EntitiesHaveSameImportantProperties(entity, persistedEntity).Should().BeTrue();
            }

            public void UpdateIsCalledWithCorrectParameters()
            {
                var state = CreateState(api, repository);
                var entity = CreateDirtyEntityWithNegativeId();
                var withPositiveId = CreateCleanWithPositiveIdFrom(entity);
                GetCreateFunction(api)(entity)
                    .Returns(Observable.Return(withPositiveId));

                state.Start(entity).SingleAsync().Wait();

                repository
                    .Received()
                    .Update(entity.Id, Arg.Is<TModel>(model => model.Id == withPositiveId.Id));
            }

            protected abstract BaseCreateEntityState<TModel> CreateState(ITogglApi api, IRepository<TModel> repository);

            protected abstract TModel CreateDirtyEntityWithNegativeId();

            protected abstract TModel CreateCleanWithPositiveIdFrom(TModel entity);

            protected abstract TModel CreateCleanEntityFrom(TModel entity);

            protected abstract Func<TModel, IObservable<TApiModel>> GetCreateFunction(ITogglApi api);

            protected abstract bool EntitiesHaveSameImportantProperties(TModel a, TModel b);
        }
    }
}
