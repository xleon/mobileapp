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

        [Theory]
        [MemberData(nameof(ClientExceptions))]
        public void ReturnsClientErrorTransitionWhenHttpFailsWithClientErrorException(ClientErrorException exception)
            => helper.ReturnsClientErrorTransitionWhenHttpFailsWithClientErrorException(exception);

        [Theory]
        [MemberData(nameof(ServerExceptions))]
        public void ReturnsServerErrorTransitionWhenHttpFailsWithServerErrorException(ServerErrorException reason)
            => helper.ReturnsServerErrorTransitionWhenHttpFailsWithServerErrorException(reason);

        [Fact]
        public void ReturnsUnknownErrorTransitionWhenHttpFailsWithNonApi()
            => helper.ReturnsUnknownErrorTransitionWhenHttpFailsWithNonApiException();

        [Fact]
        public void ReturnsFailTransitionWhenDatabaseOperationFails()
            => helper.ReturnsFailTransitionWhenDatabaseOperationFails();

        [Fact]
        public void ReturnsSuccessfulTransitionWhenEverythingWorks()
            => helper.ReturnsSuccessfulTransitionWhenEverythingWorks();

        [Fact]
        public void UpdateIsCalledWithCorrectParameters()
            => helper.UpdateIsCalledWithCorrectParameters();

        public static object[] ClientExceptions()
            => new[]
            {
                new object[] { new BadRequestException() },
                new object[] { new UnauthorizedException() },
                new object[] { new PaymentRequiredException() },
                new object[] { new ForbiddenException() },
                new object[] { new NotFoundException() },
                new object[] { new ApiDeprecatedException() },
                new object[] { new RequestEntityTooLargeException() },
                new object[] { new ClientDeprecatedException() },
                new object[] { new TooManyRequestsException() }
            };

        public static object[] ServerExceptions()
            => new[]
            {
                new object[] { new InternalServerErrorException() },
                new object[] { new BadGatewayException() },
                new object[] { new GatewayTimeoutException() },
                new object[] { new HttpVersionNotSupportedException() },
                new object[] { new ServiceUnavailableException() }
            };

        public interface IStartMethodTestHelper
        {
            void ReturnsFailTransitionWhenEntityIsNull();
            void ReturnsClientErrorTransitionWhenHttpFailsWithClientErrorException(ClientErrorException exception);
            void ReturnsServerErrorTransitionWhenHttpFailsWithServerErrorException(ServerErrorException exception);
            void ReturnsUnknownErrorTransitionWhenHttpFailsWithNonApiException();
            void ReturnsFailTransitionWhenDatabaseOperationFails();
            void ReturnsSuccessfulTransitionWhenEverythingWorks();
            void UpdateIsCalledWithCorrectParameters();
        }

        internal abstract class TheStartMethod<TModel, TApiModel> : BasePushEntityStateTests<TModel, TApiModel>, IStartMethodTestHelper
            where TModel : class, IBaseModel, IDatabaseSyncable, TApiModel
        {
            private ITogglApi api;
            private IRepository<TModel> repository;
    
            public TheStartMethod()
                : this(Substitute.For<ITogglApi>(), Substitute.For<IRepository<TModel>>())
            {
            }

            private TheStartMethod(ITogglApi api, IRepository<TModel> repository)
                : base(api, repository)
            {
                this.api = api;
                this.repository = repository;
            }

            public void ReturnsSuccessfulTransitionWhenEverythingWorks()
            {
                var state = createCreateState(api, repository);
                var entity = CreateDirtyEntityWithNegativeId();
                var clean = CreateCleanEntityFrom(entity);
                var withPositiveId = CreateCleanWithPositiveIdFrom(entity);
                getCreateFunction(api)(Arg.Any<TModel>())
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
                var state = createCreateState(api, repository);
                var entity = CreateDirtyEntityWithNegativeId();
                var withPositiveId = CreateCleanWithPositiveIdFrom(entity);
                getCreateFunction(api)(entity)
                    .Returns(Observable.Return(withPositiveId));

                state.Start(entity).SingleAsync().Wait();

                repository
                    .Received()
                    .Update(entity.Id, Arg.Is<TModel>(model => model.Id == withPositiveId.Id));
            }

            private BaseCreateEntityState<TModel> createCreateState(ITogglApi api, IRepository<TModel> repository)
                => CreateState(api, repository) as BaseCreateEntityState<TModel>;

            private Func<TModel, IObservable<TApiModel>> getCreateFunction(ITogglApi api)
                => GetApiCallFunction(api);

            protected abstract bool EntitiesHaveSameImportantProperties(TModel a, TModel b);    
        }
    }
}
