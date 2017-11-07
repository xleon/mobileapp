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
    public abstract class BasePushEntityStateTests<TModel, TApiModel>
        where TModel : class, IBaseModel, IDatabaseSyncable, TApiModel
    {
        private ITogglApi api;
        private IRepository<TModel> repository;

        public BasePushEntityStateTests(ITogglApi api, IRepository<TModel> repository)
        {
            this.api = api;
            this.repository = repository;
        }

        [Fact]
        public void ReturnsFailTransitionWhenEntityIsNull()
        {
            var state = CreateState(api, repository);

            var transition = state.Start(null).SingleAsync().Wait();
            var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

            transition.Result.Should().Be(state.UnknownError);
            parameter.Reason.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void ReturnsClientErrorTransitionWhenHttpFailsWithClientErrorException(ClientErrorException exception)
        {
            var state = CreateState(api, repository);
            var entity = CreateDirtyEntityWithNegativeId();
            PrepareApiCallFunctionToThrow(exception);

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

            transition.Result.Should().Be(state.ClientError);
            parameter.Reason.Should().BeAssignableTo<ClientErrorException>();
        }

        [Fact]
        public void ReturnsServerErrorTransitionWhenHttpFailsWithServerErrorException(ServerErrorException exception)
        {
            var state = CreateState(api, repository);
            var entity = CreateDirtyEntityWithNegativeId();
            PrepareApiCallFunctionToThrow(exception);

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

            transition.Result.Should().Be(state.ServerError);
            parameter.Reason.Should().BeAssignableTo<ServerErrorException>();
        }

        [Fact]
        public void ReturnsUnknownErrorTransitionWhenHttpFailsWithNonApiException()
        {
            var state = CreateState(api, repository);
            var entity = CreateDirtyEntityWithNegativeId();
            PrepareApiCallFunctionToThrow(new TestException());

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

            transition.Result.Should().Be(state.UnknownError);
            parameter.Reason.Should().BeOfType<TestException>();
        }

        [Fact]
        public void ReturnsFailTransitionWhenDatabaseOperationFails()
        {
            var state = CreateState(api, repository);
            var entity = CreateDirtyEntityWithNegativeId();
            PrepareDatabaseFunctionToThrow(new TestException());

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<(Exception Reason, TModel)>)transition).Parameter;

            transition.Result.Should().Be(state.UnknownError);
            parameter.Reason.Should().BeOfType<TestException>();
        }

        protected abstract BasePushEntityState<TModel> CreateState(ITogglApi api, IRepository<TModel> repository);

        protected abstract TModel CreateDirtyEntityWithNegativeId();

        protected abstract TModel CreateCleanWithPositiveIdFrom(TModel entity);

        protected abstract TModel CreateCleanEntityFrom(TModel entity);

        protected abstract void PrepareApiCallFunctionToThrow(Exception e);

        protected abstract void PrepareDatabaseFunctionToThrow(Exception e);
    }
}
